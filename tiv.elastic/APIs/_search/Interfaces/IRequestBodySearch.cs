namespace tiv.elasticClient.APIs._search.Interfaces
{
    /// <summary>
    /// Interface that represents
    /// </summary>
    public interface IRequestBodySearch
    {
        // Waiting on Bug #1018 from RestSharp
        // https://github.com/restsharp/RestSharp/issues/1018 
        // so that nulls can be ignored by default via custom PocoSerializationStrategy
        //int? from { get; set; }
        int? size { get; set; }
        IQueryDSL query { get; set; }
    }
}
