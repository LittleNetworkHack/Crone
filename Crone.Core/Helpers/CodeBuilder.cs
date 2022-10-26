namespace Crone;
public sealed class CodeBuilder
{
    private const byte IndentSize = 4;
    private byte _indent;
    private bool _indentPending = true;

    private readonly StringBuilder _stringBuilder = new StringBuilder();

    public int Length => _stringBuilder.Length;

    public CodeBuilder Append(string value)
    {
        DoIndent();
        _stringBuilder.Append(value);
        return this;
    }

    public CodeBuilder Append(char value)
    {
        DoIndent();
        _stringBuilder.Append(value);
        return this;
    }

    public CodeBuilder Append(IEnumerable<string> value)
    {
        DoIndent();
        foreach (var item in value)
        {
            _stringBuilder.Append(item);
        }
        return this;
    }

    public CodeBuilder Append(IEnumerable<char> value)
    {
        DoIndent();
        foreach (var item in value)
        {
            _stringBuilder.Append(item);
        }
        return this;
    }

    public CodeBuilder AppendLine()
    {
        AppendLine(string.Empty);
        return this;
    }

    public CodeBuilder AppendLine(string value)
    {
        if (value.Length != 0)
        {
            DoIndent();
        }
        _stringBuilder.AppendLine(value);
        _indentPending = true;
        return this;
    }

    public CodeBuilder AppendLines(string value, bool skipFinalNewline = false)
    {
        var first = true;
        string line;
        using var reader = new StringReader(value);
        while ((line = reader.ReadLine()) != null)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                AppendLine();
            }
            if (line.Length != 0)
            {
                Append(line);
            }
        }
        if (!skipFinalNewline)
        {
            AppendLine();
        }
        return this;
    }

    public CodeBuilder AppendIf(bool condition, string value)
    {
        if (condition)
        {
            Append(value);
        }
        return this;
    }

    public CodeBuilder AppendLineIf(bool condition, string value)
    {
        if (condition)
        {
            AppendLine(value);
        }
        return this;
    }

    public CodeBuilder Clear()
    {
        _stringBuilder.Clear();
        _indent = 0;
        return this;
    }

    public CodeBuilder IncrementIndent()
    {
        _indent++;
        return this;
    }
    public CodeBuilder DecrementIndent()
    {
        if (_indent > 0)
        {
            _indent--;
        }
        return this;
    }

    public IDisposable Indent()
    {
        return new Indenter(this);
    }
    public IDisposable Indent(string open, string close)
    {
        return new BlockIndenter(this, open, close);
    }

    public IDisposable SuspendIndent()
    {
        return new IndentSuspender(this);
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }

    private void DoIndent()
    {
        if (_indentPending && _indent > 0)
        {
            _stringBuilder.Append(' ', _indent * IndentSize);
        }
        _indentPending = false;
    }

    private sealed class Indenter : IDisposable
    {
        private readonly CodeBuilder _stringBuilder;

        public Indenter(CodeBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _stringBuilder.IncrementIndent();
        }

        public void Dispose()
        {
            _stringBuilder.DecrementIndent();
        }
    }

    private sealed class BlockIndenter : IDisposable
    {
        private readonly string _open;
        private readonly string _close;
        private readonly CodeBuilder _stringBuilder;

        public BlockIndenter(CodeBuilder stringBuilder, string open, string close)
        {
            _open = open;
            _close = close;
            _stringBuilder = stringBuilder;
            _stringBuilder.AppendLine(_open);
            _stringBuilder.IncrementIndent();
        }

        public void Dispose()
        {
            _stringBuilder.DecrementIndent();
            _stringBuilder.AppendLine(_close);
        }
    }

    private sealed class IndentSuspender : IDisposable
    {
        private readonly CodeBuilder _stringBuilder;
        private readonly byte _indent;

        public IndentSuspender(CodeBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _indent = _stringBuilder._indent;
            _stringBuilder._indent = 0;
        }

        public void Dispose()
        {
            _stringBuilder._indent = _indent;
        }
    }
}
