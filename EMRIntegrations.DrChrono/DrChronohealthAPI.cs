//    Copyright 2014 athenahealth, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License"); you
//   may not use this file except in compliance with the License.  You
//   may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
//   implied.  See the License for the specific language governing
//   permissions and limitations under the License.

using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;
using System.Web;

namespace EMRIntegrations.DrChrono
{
    /// <summary>
    ///   This class abstracts away the HTTP connection and basic authentication from API calls.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     When an object of this class is constructed, it attempts to authenticate (using basic
    ///     authentication) using the key, secret, and version specified.  It stores the access token
    ///     for later use.
    ///   </para>
    ///   <para>
    ///     Whenever any of the HTTP request methods are called (GET, POST, etc.), the arguments are
    ///     converted into the proper form for the request.  The result is decoded from JSON and
    ///     returned as either a JSONObject or JSONArray.
    ///   </para>
    ///   <para>
    ///     The HTTP request methods each have three signatures corresponding to common ways of making
    ///     requests: (1) just a URL, (2) URL with parameters, (3) URL with parameters and headers.
    ///     Each of these methods prepends the specified API version to the URL.  If the practice ID
    ///     is set, it is also added.
    ///   </para>
    ///   <para>
    ///     If an API call returns 401 Not Authorized, a new access token is obtained and the request
    ///     is retried.
    ///   </para>
    /// </remarks>
    public class APIConnection
    {
        /// <summary>
        ///   Store for the current Practice ID.  If it is set to the empty string, it will not be
        ///   included in URLs.
        /// </summary>
        public string PracticeID { get; set; }

        private string code;
        private string grant_type;
        private string redirect_uri;
        private string client_id;
        private string client_secret;
        private string access_token;
        private string refresh_token;
        private int expiration_in;


        private string baseUrl;
        private string version;

        // This gets used quite a bit.
        static private Encoding UTF8 = System.Text.Encoding.GetEncoding("utf-8");

        /// <summary>
        ///   Connect to the specified API version using key and secret.
        /// </summary>
        /// <param name="version">The API version to access.</param>
        /// <param name="key">The client key (also known as ID)</param>
        /// <param name="secret">The client secret</param>
        /// <param name="practiceid">The practice ID to use</param>
        public APIConnection(string api_baseUrl, string access_token)
        {
            this.baseUrl = api_baseUrl;
            this.access_token = access_token;
            //this.code = api_code;
            //this.grant_type = api_grant_type;
            //this.redirect_uri = api_redirect_uri;
            //this.client_id = api_client_id;
            //this.client_secret = api_client_secret;

            //AccessTokens tokens = GetTokens();
            //access_token = tokens.AccessToken;
            //refresh_token = tokens.RefreshToken;
            //expiration_in = tokens.Expiration;

            authenticate();
        }

        //private AccessTokens GetTokens()
        //{
        //    try
        //    {
        //        string AccessBody = "code={0}&grant_type=authorization_code&redirect_uri={1}&client_id={2}&client_secret={3}";

        //        var accessTokenRequestBody = string.Format(AccessBody, code, WebUtility.UrlEncode(redirect_uri), client_id, client_secret);
                
        //        AccessTokens tokens = null;
        //        var request = (HttpWebRequest)WebRequest.Create(PathJoin(this.baseUrl, "/o/token/"));
        //        request.Method = "POST";
        //        request.Accept = "application/json";
        //        request.ContentType = "application/x-www-form-urlencoded";

        //        request.ContentLength = accessTokenRequestBody.Length;

        //        //using (Stream requestStream = request.GetRequestStream())
        //        //{
        //        //    StreamWriter writer = new StreamWriter(requestStream);
        //        //    writer.Write(accessTokenRequestBody);
        //        //    writer.Close();
        //        //}

        //        var response = (HttpWebResponse)request.GetResponse();

        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            var reader = new StreamReader(responseStream);
        //            string json = reader.ReadToEnd();
        //            reader.Close();
        //            tokens = JsonConvert.DeserializeObject(json, typeof(AccessTokens)) as AccessTokens;
        //        }

        //        return tokens;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        ///   Perform the steps of basic authentication
        /// </summary>
        private void authenticate()
        {
            String url = PathJoin(this.baseUrl, "/o/token/");

            //// Create a new request
            //WebRequest request = WebRequest.Create(url);
            //request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";

            //// Set the Authorization header for basic auth
            //string auth = System.Convert.ToBase64String(UTF8.GetBytes(string.Format("{0}:{1}:{2}", code, client_id, client_secret)));
            //request.Headers["Authorization"] = string.Format("Basic {0}", auth);

            //Dictionary<string, string> parameters = new Dictionary<string, string>()
            //{
            //    { "redirect_uri",redirect_uri },
            //    { "grant_type", grant_type },
            //};

            //// Encode and write the parameters to the request
            //byte[] content = UTF8.GetBytes(UrlEncode(parameters));
            //Stream writer = request.GetRequestStream();
            //writer.Write(content, 0, content.Length);
            //writer.Close();

            //// Get and decode the response
            //WebResponse response = request.GetResponse();
            //Stream receive = response.GetResponseStream();
            //StreamReader reader = new StreamReader(receive, UTF8);
            //string authorization = reader.ReadToEnd();
            //dynamic jsonResponse = JsonConvert.DeserializeObject(authorization);

            //// Don't forget to close the response!

            //// Grab the token!
            //this.access_token = jsonResponse["access_token"].ToString();// authorization["access_token"];
            //this.refresh_token = jsonResponse["refresh_token"].ToString();// authorization["refresh_token"];
            //response.Close();

            this.access_token = "JHza2o8Lwoe57gQD2UdX8iqIhNLa5b";
            this.refresh_token = "j2j4R68sh9ZQon3jcuZwK02BMBU0ZN";
        }

        /// <summary>
        ///   Convert parameters into a URL query string.
        /// </summary>
        /// <param name="parameters">The parameters to encode</param>
        /// <returns>The URL query string</returns>
        static public string UrlEncode(Dictionary<string, string> parameters)
        {

            return string.Join("&", parameters.Select(
             kvp => WebUtility.HtmlEncode(kvp.Key) + "=" + WebUtility.HtmlEncode(kvp.Value)
           ).ToList());
        }

        /// <summary>
        ///   Join arguments into a valid URL.
        /// </summary>
        /// <param name="args">Any number of strings to join</param>
        /// <returns>The resulting URL</returns>
        static private string PathJoin(params string[] args)
        {
            // Trim slashes, filter out empties, join by slashes
            return string.Join("/", args
                               .Select(arg => arg.Trim(new char[] { '/' }))
                               .Where(arg => !String.IsNullOrEmpty(arg))
            );
        }

        /// <summary>
        ///   Make the API call.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method abstracts away the streams, readers, and writers necessary to format the
        ///     HTTP request.  It also adds in the Authorization header and token.
        ///   </para>
        /// </remarks>
        /// <param name="request">The request to format and send</param>
        /// <param name="body">The parameters that will be written to the request body</param>
        /// <param name="headers">The headers that will be added to the request</param>
        /// <param name="secondcall">True if this is the retried call</param>
        /// <returns>The JSON-decoded response</returns>
        private object AuthorizedCall(WebRequest request, Dictionary<string, string> body, Dictionary<string, string> headers, bool secondcall = false)
        {
            // First add the auth header, then update with the rest of them.
            request.Headers["Authorization"] = string.Format("Bearer {0}", this.access_token);
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> kvp in body)
                {
                    request.Headers[kvp.Key] = kvp.Value;
                }
            }

            // Write the body parameters, if there are any.
            if (body != null)
            {
                byte[] content = UTF8.GetBytes(UrlEncode(body));
                Stream writer = request.GetRequestStream();
                writer.Write(content, 0, content.Length);
                writer.Close();
            }

            // Try reading the response, assuming it's good.  If not, read the error response.
            StreamReader reader;
            string reply;
            try
            {
                Thread.Sleep(500);
                WebResponse response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), UTF8);
                reply = reader.ReadToEnd();
                response.Close();

                //return object.Parse(reply);                
                dynamic jsonResponse = JsonConvert.DeserializeObject(reply);
                return jsonResponse;
            }
            catch (WebException wex)
            {
                Stream exceptionRespose = wex.Response.GetResponseStream();
                exceptionRespose.Flush();
                reader = new StreamReader(exceptionRespose, UTF8);
                reply = reader.ReadToEnd();

                // If we get a 401 Not Authorized, re-authenticate and try again.
                if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.Unauthorized && !secondcall)
                {
                    this.authenticate();

                    // We can't reopen the stream from the same request twice, so create a new one that's identical and send that.
                    WebRequest retry = WebRequest.Create(request.RequestUri);
                    retry.ContentType = request.ContentType;
                    retry.Method = request.Method;
                    return this.AuthorizedCall(retry, body, headers, true);
                }
                //  return object.Parse(reply);                
                dynamic jsonResponse = JsonConvert.DeserializeObject(reply);
                return jsonResponse;
            }
        }

        /// <summary>
        ///   Perform a GET request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <returns>The JSON-decoded response</returns>
        public object GET(string path)
        {
            return GET(path, null, null);
        }

        /// <summary>
        ///   Perform a GET request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The JSON-decoded response</returns>
        public object GET(string path, Dictionary<string, string> parameters)
        {
            return GET(path, parameters, null);
        }

        /// <summary>
        ///   Perform a GET request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="headers">The request headers</param>
        /// <returns>The JSON-decoded response</returns>
        public object GET(string path, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            string url = PathJoin(this.baseUrl, path);
            string query = "";
            if (parameters != null)
            {
                query = "?" + UrlEncode(parameters);
            }

            WebRequest request = WebRequest.Create(url + query);
            request.Method = "GET";

            return AuthorizedCall(request, null, headers);
        }


        /// <summary>
        ///   Perform a POST request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <returns>The JSON-decoded response</returns>
        public object POST(string path)
        {
            return POST(path, null, null);
        }

        /// <summary>
        ///   Perform a POST request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The JSON-decoded response</returns>
        public object POST(string path, Dictionary<string, string> parameters)
        {
            return POST(path, parameters, null);
        }

        /// <summary>
        ///   Perform a POST request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="headers">The request headers</param>
        /// <returns>The JSON-decoded response</returns>
        public object POST(string path, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            string url = PathJoin(this.baseUrl, this.version, this.PracticeID, path);
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            return AuthorizedCall(request, parameters, headers);
        }


        /// <summary>
        ///   Perform a PUT request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <returns>The JSON-decoded response</returns>
        public object PUT(string path)
        {
            return PUT(path, null, null);
        }

        /// <summary>
        ///   Perform a PUT request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The JSON-decoded response</returns>
        public object PUT(string path, Dictionary<string, string> parameters)
        {
            return PUT(path, parameters, null);
        }

        /// <summary>
        ///   Perform a PUT request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="headers">The request headers</param>
        /// <returns>The JSON-decoded response</returns>
        public object PUT(string path, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            string url = PathJoin(this.baseUrl, this.version, this.PracticeID, path);
            WebRequest request = WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded";

            return AuthorizedCall(request, parameters, headers);
        }


        /// <summary>
        ///   Perform a DELETE request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <returns>The JSON-decoded response</returns>
        public object DELETE(string path)
        {
            return DELETE(path, null, null);
        }

        /// <summary>
        ///   Perform a DELETE request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <returns>The JSON-decoded response</returns>
        public object DELETE(string path, Dictionary<string, string> parameters)
        {
            return DELETE(path, parameters, null);
        }

        /// <summary>
        ///   Perform a DELETE request.
        /// </summary>
        /// <param name="path">The URI to access</param>
        /// <param name="parameters">The request parameters</param>
        /// <param name="headers">The request headers</param>
        /// <returns>The JSON-decoded response</returns>
        public object DELETE(string path, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            string url = PathJoin(this.baseUrl, this.version, this.PracticeID, path);
            string query = "";
            if (parameters != null)
            {
                query = "?" + UrlEncode(parameters);
            }
            WebRequest request = WebRequest.Create(url + query);
            request.Method = "DELETE";

            return AuthorizedCall(request, null, headers);
        }

        /// <summary>
        ///   Gets the current access token.
        /// </summary>
        /// <returns>The access token</returns>
        public string GetToken()
        {
            return this.access_token;
        }

        public void SetToken(string token)
        {
            this.access_token = token;
        }
    }

    // The body of the response from GetTokens is a JSON object that 
    // contains the following properties (and a couple of others
    // that we're not capturing).

    [JsonObject(MemberSerialization.OptIn)]
    class AccessTokens
    {
        [JsonProperty("expires_in")]
        public int Expiration { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
