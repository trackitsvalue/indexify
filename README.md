# tiv.elasticClient
This is the Elastic RESTful API Client Library used by indexify. It can be used seperately in your own projects and is available on nuget as https://www.nuget.org/packages/tiv.elasticClient/.


# indexify

This command line tool will reindex a single or series of source Elastic indexes based on the passed index pattern into a destination index.

***INDEXIFY "url" "/src=index-pattern" "/dest=index" | /dest-is-src-minus=NUMBER | /dest-is-src-plus=STRING | /no-dest-delete-source
            [/ba=USERNAME:PASSWORD] [/v] [/olf=ouput-logfile] [/copyonly] [/dryrun] [/scrollcount=NUMBER]***

| Parameter                  | Description                                            |
|----------------------------|--------------------------------------------------------|
| "url"                      | The full url to find the elastic instance at, i.e.: https://servername.domain.com:9200 |
| "/src=index-pattern"       | The source index or pattern to search for, can include *'s for wildcards. |
| "/dest=index-name"         | The destination index name to write to. |
|                            | OR |
| /dest-is-src-minus=NUMBER  | The destination index name should be the source index name minus some number of  characters. |
|                            | OR |
| /dest-is-src-plus=STRING   | The destination index name should be the source index name plus the passed string. |
|                            | OR |
| /no-dest-delete-source     | Just delete the source indexes that match the specified criteria. |
| /ba=USERNAME:PASSWORD      | The basic authentication user and password if needed. |
| /lf=logfile                | The log file to write results of the operations into, i.e.: ***C:\reindexify\logs.log****  It can also contain a datetime format for a string format parameter 0 which can be used to insert a format of DateTime.Now into the file name or directory structure,  i.e.: c:\reindexify\logs-{0:yyyy-MM-dd}.log
| /v                         | If specified, display verbose logging to the console.
| /copyonly                  | If specified, the index rewriting will not delete the source index on complete and successful copying of all documents. ***NOTE: Not relevant when option is /no-dest-delete-source.*** |
| /scrollcount=NUMBER        | If specified, the index rewriting will NOT use the Reindex API and will instead use the Search API with scrolling and the Bulk API for the copy and also potentially the deletes. This is a better option if the document counts are causing timeout exceptions during reindex operations. ***NOTE: Not relevant when option is /no-dest-delete-source.***
| /dryrun                    | If specified, this will be a dry run only and no changes will be made. |
