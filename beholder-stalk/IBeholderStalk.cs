namespace beholder_stalk
{
  using System.Threading.Tasks;

  public interface IBeholderStalk
  {
    /// <summary>
    /// Gets the keyboard HID
    /// </summary>
    Keyboard Keyboard
    {
      get;
    }

    /// <summary>
    /// Gets the mouse HID
    /// </summary>
    Mouse Mouse
    {
      get;
    }

    /// <summary>
    /// Instructs the Stalk to connect to the nexus
    /// </summary>
    /// <returns></returns>
    Task Connect();

    /// <summary>
    /// Publish a payload object 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <param name="retain"></param>
    /// <returns></returns>
    Task Publish<T>(string topic, T payload, bool retain = false);
  }
}