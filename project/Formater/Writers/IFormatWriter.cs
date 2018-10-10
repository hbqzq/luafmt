
namespace lfmt
{
    public interface IFormatWriter
    {
        void Append(string s);
        void Close();
    }
}
