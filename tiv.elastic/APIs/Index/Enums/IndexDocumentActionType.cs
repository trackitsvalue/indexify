namespace tiv.elastic.APIs.Index.Enums
{
    public class IndexDocumentActionType
    {
        // Create a document only if the document does not already exist.
        public const string Create = "create";

        // Create a new document or replace an existing document. 
        public const string Index = "index";

        // Do a partial update on a document.
        public const string Update = "update";

        // Delete a document.
        public const string Delete = "delete";
    }
}
