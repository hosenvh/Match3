using System;

namespace Match3.CloudSave
{

    public class ExpectedCloudDataNotExistsException : Exception
    {
        public ExpectedCloudDataNotExistsException()
        {
        }

        public ExpectedCloudDataNotExistsException(string key)
            : base($"Expected Data In Cloud Storage Doesn't Exists', key : {key}")
        {
        }
    }

}
