using System.Text;

namespace dotacp.generator
{
    public static class StringBuilderExt
    {
        public static StringBuilder AppendLineLf(this StringBuilder sb, string value = null)
        {
            if (value != null)
                sb.Append(value);
            return sb.Append('\n');
        }
    }
}
