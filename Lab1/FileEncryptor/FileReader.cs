namespace FileEncryptor;

public class FileReader(string path)
{
    private readonly string _path = path;
    
    public event ProgressChangedHandler? ProgressChanged;
    
    public async IAsyncEnumerable<string> GetDataAsync()
    {
        if (!File.Exists(_path))
        {
            yield break;
        }

        long bytesRead = 0L;
        long fileSize = new FileInfo(_path).Length;
        
        using (var streamReader = new StreamReader(_path))
        {
            while (!streamReader.EndOfStream)
            {
                string line = await streamReader.ReadLineAsync() ?? String.Empty;

                bytesRead += streamReader.CurrentEncoding.GetByteCount(line);
                ProgressChanged?.Invoke(this, (int)(bytesRead / fileSize * 100));
                
                yield return line;
            }
        }
    }
}

public delegate void ProgressChangedHandler(object sender, int progress);