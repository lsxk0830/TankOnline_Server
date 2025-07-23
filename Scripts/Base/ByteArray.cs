using System.Text;

public class ByteArray
{
    /// <summary>
    /// 默认大小
    /// </summary>
    private const int DEFAULF_SIZE = 2048;

    /// <summary>
    /// 缓冲区
    /// </summary>
    public byte[] bytes;

    /// <summary>
    /// 缓冲区容量
    /// </summary>
    public int capacity = 0;

    /// <summary>
    /// 初始长度
    /// </summary>
    public int initSize;

    /// <summary>
    /// 可从缓冲区读取的位置，缓冲区【有效数据】的起始位置 [0][3][c][a][t][0][2][h][i] readIdx = 5【可能为】
    /// </summary>
    public int readIdx = 0;

    /// <summary>
    /// 可从缓冲区写的位置,缓冲区【有效数据】的末尾 [0][3][c][a][t][0][2][h][i] writeIdx = 9
    /// </summary>
    public int writeIdx = 0;

    /// <summary>
    /// 缓冲区还可容纳的字节数
    /// </summary>
    public int remain { get { return capacity - writeIdx; } }

    /// <summary>
    /// 数据长度
    /// </summary>
    public int length { get { return writeIdx - readIdx; } }

    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    public ByteArray(int size = DEFAULF_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }

    /// <summary>
    /// 动态扩展缓冲区
    /// </summary>
    /// <param name="size">需要的最小容量</param>
    public void ReSize(int size)
    {
        if (size < length) return; // 新的尺寸要比数据长度大
        if (size < initSize) return; // 新的尺寸要比原来的大
        int n = capacity;
        while (n < size) n *= 2;
        capacity = n;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, length);
        bytes = newBytes;
        writeIdx = length;
        readIdx = 0;
    }

    /// <summary>
    /// 检查并移动数据
    /// </summary>
    public void CheckAndMoveBytes()
    {
        if ((writeIdx > capacity * 0.8 || length > capacity * 0.5) && readIdx > capacity * 0.3)
        {
//#if DEBUG
//            Console.ForegroundColor = ConsoleColor.Red; // 设置为红色
//            Console.WriteLine($"移动前:{readIdx},Write:{writeIdx},Length:{length}.已解析数据占用过多时移动.");
//            Console.ResetColor();
//#endif
            MoveBytes();
        }
    }

    /// <summary>
    /// 移动数据
    /// </summary>
    public void MoveBytes()
    {
        if (length > 0)
            Array.Copy(bytes, readIdx, bytes, 0, length);
        writeIdx = length;
        readIdx = 0;
    }

    public void ResetBytes()
    {
        bytes = new byte[initSize];
        capacity = initSize;
        readIdx = 0;
        writeIdx = 0;
    }

    #region 读写

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="offset">开始写入缓冲区位置</param>
    /// <param name="count">写的数据个数</param>
    public int Write(byte[] bs, int offset, int count)
    {
        if (remain < count)
            ReSize(length + count);
        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    public int Read(byte[] bs, int offset, int count)
    {
        count = Math.Min(count, length);
        Array.Copy(bytes, readIdx, bs, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;
    }

    /// <summary>
    /// 读取Int16
    /// </summary>
    public Int16 ReadInt16()
    {
        if (length < 2) return 0;
        Int16 ret = (Int16)(bytes[readIdx] << 8 | bytes[readIdx + 1]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    /// <summary>
    /// 读取Int32
    /// </summary>
    public Int32 ReadInt32()
    {
        if (length < 4) return 0;
        Int32 ret = (Int32)(bytes[readIdx + 0] << 24 |
                            bytes[readIdx + 1] << 16 |
                            bytes[readIdx + 2] << 8 |
                            bytes[readIdx + 3]);
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }

    #endregion 读写

    #region 调试

    public override string ToString()
    {
        return Encoding.UTF8.GetString(bytes, readIdx, length);
    }

    public string Debug()
    {
        return $"readIdx-{readIdx},writeIdx-{writeIdx},bytes-{Encoding.UTF8.GetString(bytes, 0, bytes.Length)}";
    }
    #endregion
}
