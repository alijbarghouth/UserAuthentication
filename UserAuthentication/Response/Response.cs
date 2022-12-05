namespace UserAuthentication.Response
{
    public class Response<T>
    {
        public T Data { get; set; }

        public Response(T data)
        {
            Data= data;
        }
    }
}
