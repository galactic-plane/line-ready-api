namespace LineReadyApi.Infrastructure
{
    public interface IEtaggable
    {
        string GetEtag();
    }
}