using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using indexify.Models;
using RestSharp.Authenticators;
using tiv.elasticClient;
using tiv.elasticClient.APIs.Index;
using tiv.elasticClient.APIs.Index.Enums;
using tiv.elasticClient.APIs._bulk;
using tiv.elasticClient.APIs._bulk.ExtensionMethods;
using tiv.elasticClient.APIs._cat;
using tiv.elasticClient.APIs._reindex;
using tiv.elasticClient.APIs._reindex.Models;
using tiv.elasticClient.APIs._search;
using tiv.elasticClient.APIs._search.Models;
using tiv.elasticClient.APIs._search.Models.QueryDLS;
using tiv.elasticClient.Exceptions;
using tiv.elasticClient.ExtensionFunctions;
using tiv.elasticClient.APIs._mapping;
using tiv.elasticClient.APIs._settings;
using System.Threading.Tasks;
using tiv.elasticClient.APIs._cat.Models;
using System.Collections.Concurrent;

namespace indexify
{
    static class indexify
    {
        private static ConcurrentBag<ReindexResultModel> _results = new ConcurrentBag<ReindexResultModel>();
        private static string _currentResourceCall = string.Empty;
        private static IAuthenticator _IAuth;
        private static CommandLineModel _commandLineModel;
        private static ConcurrentDictionary<string, bool> _indexExists = new ConcurrentDictionary<string, bool>();
        private static object _semaphore = new object();

        public static void Execute(CommandLineModel cmdLineModel)
        {
            long elaspsedTotalTime = 0;

            try
            {
                SetupClassProperties(cmdLineModel);

                elaspsedTotalTime = ProcessIndexes();
            }
            catch (RestCallException ex)
            {
                VerboseLogLine($"ERROR: RESTFul Call Failed calling '{_currentResourceCall}'\n");
                VerboseLogLine($"Http Status Code: {ex.StatusCode}\n");
                VerboseLogLine($"Http Status Description: {ex.StatusDescription}\n");
                if (ex.InnerException != null)
                {
                    VerboseLogLine($"Exception: {ex.InnerException.Message}\n");
                }
                VerboseLogLine("                                                                             : (");
            }
            catch (Exception ex)
            {
                VerboseLogLine("ERROR: Unexpected Exception\n");
                VerboseLogLine($"{ex.Message}\n");
                VerboseLogLine("                                                                             : (");
            }
            try
            {
                LogResults();
            }
            catch
            {
                
            }
            VerboseLogLine($"Operation Completed in {elaspsedTotalTime}ms.");
        }

        private static void SetupClassProperties(CommandLineModel cmdLineModel)
        {
            _commandLineModel = cmdLineModel;
            if (cmdLineModel.UseBasicAuth)
            {
                _IAuth = new HttpBasicAuthenticator(cmdLineModel.UserName, cmdLineModel.Password);
            }
            // See README.MD for more information
            //SimpleJson.CurrentJsonSerializerStrategy = new PocoJsonSerializerStrategy();
            if (cmdLineModel.Verbose)
            {
                VerboseLogLine("Executing INDEXIFY with the following parameters:\n");
                VerboseLogLine($"Source URL: {cmdLineModel.Url} ");
                VerboseLogLine($"Source Index Pattern: {cmdLineModel.SourceIndexPattern}");
                
                if (!cmdLineModel.LogFile.FullName.IsNullOrEmpty())
                {
                    VerboseLogLine($"Log File: {cmdLineModel.LogFile}");
                }

                if (cmdLineModel.UseBasicAuth)
                {
                    VerboseLogLine($"Base Auth User: {cmdLineModel.UserName}");
                }

                if (_commandLineModel.NoDestinationJustDelete)
                {
                    VerboseLogLine($"WARNING: Source Indexes will be deleted with NO DATA COPY!");
                }
                else
                {
                    if (!_commandLineModel.DestinationIndex.IsNullOrEmpty())
                    {
                        VerboseLogLine($"Destination Index Name: {cmdLineModel.DestinationIndex}");
                    }
                    else
                    {
                        if (_commandLineModel.SourceMinusNumChars > 0)
                        {
                            VerboseLogLine(
                                $"Destination Index will be source index name minus {_commandLineModel.SourceMinusNumChars} characters.");
                        }
                        if (!_commandLineModel.SourcePlusString.IsNullOrEmpty())
                        {
                            VerboseLogLine(
                                $"Destination Index will be source index name plus '{_commandLineModel.SourcePlusString}' appended to index name.");
                        }
                    }

                    if (_commandLineModel.CopyOnly)
                    {
                        VerboseLogLine("Source index WILL NOT be deleted upon completely successful reindexing.");
                    }
                }
                if (_commandLineModel.DryRun)
                {
                    VerboseLogLine("Dry Run only.");
                }
                if (_commandLineModel.ScrollCount > 0)
                {
                    VerboseLogLine($"Scroll Count: {_commandLineModel.ScrollCount}");
                }
                VerboseLogLine("");
            }
        }

        private static void VerboseLogLine(string logString)
        {
            Console.WriteLine(logString);
        }

        private static void VerboseLog(string logString)
        {
            Console.Write(logString);
        }

        private static void LogResults()
        {
            if (_results.Count > 0)
            {
                if (_commandLineModel.LogFile != null)
                {
                    Directory.CreateDirectory(_commandLineModel.LogFile.DirectoryName);
                    using (var logFile = File.Open(_commandLineModel.LogFile.FullName, FileMode.Create))
                    {
                        using (var textWriter = new StreamWriter(logFile))
                        {
                            foreach (var result in _results)
                            {
                                if (result.ReindexSkipped)
                                {
                                    textWriter.WriteLine($"SKIPPED INDEX '{result.IndexSource}', Reason: {result.SkippedReason}");
                                }
                                else if (_commandLineModel.NoDestinationJustDelete)
                                {
                                    textWriter.WriteLine($"DELETE '{result.IndexSource}' took {result.ActionTime} ms, Src: {result.DocsInOldIndex}, DeleteAck: {result.DeleteAcknowledged} ");
                                }
                                else if(result.DeleteIndexSkipped)
                                {
                                    textWriter.WriteLine($"REINDEX '{result.IndexSource}' => '{result.IndexDestination}' took {result.ActionTime} ms, Src: {result.DocsInOldIndex}, DestCreated: {result.DocsCreated}, DestUpdated: {result.DocsUpdated}, DestVersionConflicts: {result.DocsVersionConflicts} ");
                                }
                                else if (result.ExceptionEncountered)
                                {
                                    textWriter.WriteLine($"EXCEPTION '{result.IndexSource}' => '{result.IndexDestination}' took {result.ActionTime} ms, Src: {result.DocsInOldIndex}, DestCreated: {result.DocsCreated}, DestUpdated: {result.DocsUpdated}, DestVersionConflicts: {result.DocsVersionConflicts}, Exception: {result.ExceptionInfo} ");
                                }
                                else
                                {
                                    textWriter.WriteLine($"REINDEX + DELETE '{result.IndexSource}' => '{result.IndexDestination}' took {result.ActionTime} ms, Src: {result.DocsInOldIndex}, DestCreated: {result.DocsCreated}, DestUpdated: {result.DocsUpdated}, DestVersionConflicts: {result.DocsVersionConflicts}, DeleteAck: {result.DeleteAcknowledged} ");
                                }
                            }
                            textWriter.Flush();
                            textWriter.Close();
                        }
                    }
                }
            }
        }

        private static long ProcessIndexes()
        {
            var client = RestClientUtility.Instance.NewClient(_commandLineModel.Url, _IAuth);
            var catApi = new CatAPI();

            var indexes = catApi.Get(client, _commandLineModel.SourceIndexPattern, out _currentResourceCall);
            indexes = indexes.OrderBy(m => m.index).ToList();

            if (_commandLineModel.Verbose)
            {
                VerboseLogLine($"Call to '{_currentResourceCall} returned {indexes.Count} indexes.");
            }

            var totalSw = new Stopwatch();
            totalSw.Start();

            Parallel.ForEach(indexes, new ParallelOptions() { MaxDegreeOfParallelism = _commandLineModel.MaxParallelism } , index =>
            {
                ReIndex(index);
            });

            totalSw.Stop();
            return totalSw.ElapsedMilliseconds;
        }

        private static void ReIndex(CatResponse index)
        {
            var client = RestClientUtility.Instance.NewClient(_commandLineModel.Url, _IAuth);

            var catApi = new CatAPI();
            var mappingApi = new MappingAPI();
            var settingsApi = new SettingsAPI();
            var reindexApi = new ReindexAPI();
            var indexApi = new IndexAPI();
            var searchAndScrollApi = new SearchAndScrollAPI();
            var bulkApi = new BulkAPI();

            var indexSw = new Stopwatch();

            string verb;

            if (_commandLineModel.NoDestinationJustDelete)
            {
                verb = _commandLineModel.DryRun ? "Deleting DryRun" : "Deleting";
            }
            else
            {
                verb = _commandLineModel.DryRun ? "Reindexing DryRun" : "Reindexing";
            }

            var rir = new ReindexRequest
            {
                conflicts = "proceed",
                source = new ReindexSource()
                {
                    index = index.index
                },
                dest = new ReindexDestination()
                {
                    index = BuildDestinationIndex(index.index),
                    op_type = "create"
                }
            };

            var reindexResultModel = new ReindexResultModel()
            {
                IndexSource = rir.source.index,
                IndexDestination = rir.dest.index
            };

            try
            {
                var skipIndex = false;

                if (!index.health.CompareNoCase("green") && !index.health.CompareNoCase("yellow"))
                {
                    reindexResultModel.ReindexSkipped = true;
                    reindexResultModel.SkippedReason = $"Index health '{index.health}'.";
                    skipIndex = true;
                }
                else if (!index.status.CompareNoCase("open"))
                {
                    reindexResultModel.ReindexSkipped = true;
                    reindexResultModel.SkippedReason = $"Index status '{index.health}'.";
                    skipIndex = true;
                }
                else if (index.docscount == 0 && !_commandLineModel.NoDestinationJustDelete)
                {
                    reindexResultModel.ReindexSkipped = true;
                    reindexResultModel.SkippedReason = "No documents in index.";
                    skipIndex = true;
                }

                if (skipIndex)
                {
                    if (_commandLineModel.DryRun || _commandLineModel.Verbose)
                    {
                        VerboseLogLine($"{verb} ({index.docscount} Docs):");
                        VerboseLogLine($"   {index.index} ! SKIPPED ({reindexResultModel.SkippedReason})");
                    }
                }
                else
                {
                    if (_commandLineModel.DryRun || _commandLineModel.Verbose)
                    {
                        VerboseLogLine($"{verb} ({index.docscount} Docs):");
                        VerboseLogLine($"   {index.index} ==> {rir.dest.index}");
                    }

                    if (!_commandLineModel.DryRun)
                    {
                        // Check whether index already exists
                        if (_commandLineModel.CopySourceIndexMappingsAndSettings)
                        {
                            lock (_semaphore)
                            {
                                if (!_indexExists.ContainsKey(rir.dest.index) && !catApi.IndexExists(client, rir.dest.index))
                                {
                                    // We need to create the destination index first with mapping + settings
                                    var mappings = mappingApi.Get(client, index.index, out _currentResourceCall);
                                    var settings = settingsApi.Get(client, index.index, out _currentResourceCall, shards: _commandLineModel.Shards, replicas: _commandLineModel.Replicas);
                                    indexApi.Create(client, rir.dest.index, settings, mappings, out _currentResourceCall);

                                    // Mark as exists
                                    _indexExists.AddOrUpdate(rir.dest.index, true, (key, oldValue) => { return true; });
                                }
                            }
                        }

                        // Can we use reindex API or do we need to scroll?
                        if (_commandLineModel.ScrollCount == 0 || _commandLineModel.ScrollCount >= index.docscount | _commandLineModel.NoDestinationJustDelete)
                        {
                            reindexResultModel.DocsInOldIndex = index.docscount;
                            indexSw.Restart();
                            if (_commandLineModel.NoDestinationJustDelete)
                            {
                                reindexResultModel.DocsDeleted = index.docscount;
                                var dir = indexApi.Delete(client, rir.source.index, out _currentResourceCall);
                                reindexResultModel.DeleteAcknowledged = dir.acknowledged;
                            }
                            else
                            {
                                var reindexResponse = reindexApi.Post(client, rir, out _currentResourceCall);
                                reindexResultModel.DocsCreated = reindexResponse.created;
                                reindexResultModel.DocsUpdated = reindexResponse.updated;
                                reindexResultModel.DocsVersionConflicts = reindexResponse.version_conflicts;

                                if (reindexResponse.created + reindexResponse.updated +
                                    reindexResponse.version_conflicts != index.docscount)
                                {
                                    reindexResultModel.DeleteIndexSkipped = true;
                                    VerboseLogLine($"WARNING: Skipped call delete index '{index.index}' as reindex document count does not match source count (DestCreated {reindexResponse.created} + DestUpdated {reindexResponse.updated} + DestVerConflict {reindexResponse.version_conflicts} != Src {index.docscount})");
                                }
                                else if (!_commandLineModel.CopyOnly)
                                {
                                    var dir = indexApi.Delete(client, rir.source.index, out _currentResourceCall);
                                    reindexResultModel.DeleteAcknowledged = dir.acknowledged;
                                }
                            }

                            indexSw.Stop();
                            reindexResultModel.ActionTime = indexSw.ElapsedMilliseconds;
                        }
                        // We need to scroll rather than reindex.
                        else
                        {
                            reindexResultModel.DocsInOldIndex = index.docscount;
                            indexSw.Restart();

                            var sasRequest = new SearchAndScrollRequest()
                            {
                                size = _commandLineModel.ScrollCount,
                                query = new MatchAllQuery()
                            };
                            var searchAndScrollResponse = searchAndScrollApi.PostSearch(client, sasRequest, rir.source.index, "5m", out _currentResourceCall);

                            while (searchAndScrollResponse.hits.hits.Count > 0)
                            {
                                var bulkRequestBuilder = new StringBuilder();
                                foreach (var hit in searchAndScrollResponse.hits.hits)
                                {
                                    bulkRequestBuilder.AddHitToBulkOperation(IndexDocumentActionType.Create, hit._source, rir.dest.index, hit._type, hit._id);
                                    if (!_commandLineModel.CopyOnly)
                                    {
                                        bulkRequestBuilder.AddHitToBulkOperation(IndexDocumentActionType.Delete, hit._source, hit._index, hit._type, hit._id);
                                    }
                                }
                                var bulkRequestBody = bulkRequestBuilder.ToString();
                                var bulkResponse = bulkApi.Post(client, bulkRequestBody, out _currentResourceCall);
                                if (bulkResponse.errors == true) reindexResultModel.ExceptionEncountered = true;

                                reindexResultModel.DocsCreated += bulkResponse.items.Where(m => m.create != null && (m.create.status == 201)).ToList().Count;
                                searchAndScrollResponse = searchAndScrollApi.PostScroll(client, "5m", searchAndScrollResponse.scroll_id, out _currentResourceCall);
                            }


                            if (reindexResultModel.DocsCreated != reindexResultModel.DocsInOldIndex)
                            {
                                reindexResultModel.DeleteIndexSkipped = true;
                                VerboseLogLine($"WARNING: Skipped call delete index '{index.index}' as document copy count does not match source count (DestCreated {reindexResultModel.DocsCreated} != Src {index.docscount})");
                            }
                            else if (!_commandLineModel.CopyOnly)
                            {
                                var dir = indexApi.Delete(client, rir.source.index, out _currentResourceCall);
                                reindexResultModel.DeleteAcknowledged = dir.acknowledged;
                            }

                            indexSw.Stop();
                            reindexResultModel.ActionTime = indexSw.ElapsedMilliseconds;
                            VerboseLogLine($"   {index.index} ==> {rir.dest.index} finished in {indexSw.ElapsedMilliseconds}ms");
                        }
                    }
                    else
                    {
                        reindexResultModel.ReindexSkipped = true;
                        reindexResultModel.SkippedReason = "DryRun is true";
                    }
                }
            }
            catch (RestCallException rex)
            {
                VerboseLogLine($"ERROR: RESTFul Call Failed calling '{_currentResourceCall}'");
                VerboseLogLine($"Http Status Code: {rex.StatusCode}");
                VerboseLogLine($"Http Status Description: {rex.StatusDescription}");
                if (rex.InnerException != null)
                {
                    VerboseLogLine($"Exception: {rex.InnerException.Message}");
                }
                VerboseLogLine("");
                reindexResultModel.ExceptionEncountered = true;
                reindexResultModel.ExceptionInfo = $"'{_currentResourceCall}' Call Failed, Status Code: {rex.StatusCode}, Status Description: {rex.StatusDescription}";
            }

            _results.Add(reindexResultModel);
        }

        private static string BuildDestinationIndex(string sourceIndex)
        {
            if (!_commandLineModel.DestinationIndex.IsNullOrEmpty()) return _commandLineModel.DestinationIndex;
            if (_commandLineModel.NoDestinationJustDelete) return "DELETED!";
            if(!_commandLineModel.DestIsSource) throw new ArgumentException("Unable to determine proper destination index name.");
            var destinationIndexName = sourceIndex;
            if (_commandLineModel.SourceMinusNumChars > 0)
            {
                destinationIndexName = destinationIndexName.Substring(0,
                    destinationIndexName.Length - _commandLineModel.SourceMinusNumChars);
            }
            if (!_commandLineModel.SourcePlusString.IsNullOrEmpty())
            {
                destinationIndexName += _commandLineModel.SourcePlusString;
            }
            return (destinationIndexName);
        }
    }
}
