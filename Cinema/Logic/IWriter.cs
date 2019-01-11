namespace Cinema
{
    interface IWriter
    {
        void SerializeToXml(string path);
        void DeSerializeToXml(string path);

    }
}
