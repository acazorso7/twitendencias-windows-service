# Windows Services

To use this code you should publish this services in IIS (Internet Information Services) or in Azure.
These services will be called from Web Application. You can find the references to these services in the file (index.html) of the project https://github.com/acazorso7/twitendencias-node-js.

Notes:
- HBase-Query/Service1.svc.cs
  - (line 23): The URL is the URL of restful service from HBase. This URL could be different depending or your enviornment. Please check.
  - (line 36): The conversion to JSON object. 
