namespace beholder_occipital_v1.Cache
{
  using System;
  using System.Threading.Tasks;

  public interface ICacheClient
  {
    Task<byte[]> Base64ByteArrayGet(string key);
    Task<bool> Base64ByteArraySet(string key, byte[] value, TimeSpan? expiry = null);

    Task<T> JsonGet<T>(string key);
    Task<bool> JsonSet<T>(string key, T value, TimeSpan? expiry = null);
  }
}