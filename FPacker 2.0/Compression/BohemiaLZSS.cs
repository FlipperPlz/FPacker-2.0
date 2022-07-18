using System.Text;

namespace FPacker.Compression;

internal static class BohemiaLZSS {
    private const int PacketFormatUncompressed = 1;
    private const byte Space = 0x20;
    
    public static byte[] Compress(byte[] data) {
        var output = new List<byte>();
        var buffer = new CompressionBuffer();

        var dataLength = data.Length;
        var readData = 0;

        //Generate and add compressed data
        while (readData < dataLength) {
            var packet = new Packet();
            readData = packet.Pack(data, readData, buffer);

            var content = packet.GetContent();

            output.AddRange(content);
        }

        //Calculate and add checksum of compressed data
        var checksum = CalculateChecksum(data);
        output.AddRange(checksum);

        //Console.WriteLine(Encoding.UTF8.GetString(Decompress(output.ToArray(), dataLength)));
        
        return output.ToArray();
    }
    
    public static uint DecompressStream( Stream input, out byte[] dst, uint expectedSize, bool useSignedChecksum) {
            var array = new char[4113];
            dst = new byte[expectedSize];
            if (expectedSize == 0U) return 0U;
            var position = input.Position;
            var num = expectedSize;
            var num2 = 0;
            var num3 = 0;
            for (var i = 0; i < 4078; i++) array[i] = ' ';
            var num4 = 4078;
            var num5 = 0;
            while (num != 0U) {
                if (((num5 >>= 1) & 256) == 0) num5 = (input.ReadByte() | 65280);
                if ((num5 & 1) != 0) {
                    var num6 = input.ReadByte();
                    num3 = ((!useSignedChecksum) ? (num3 + ((byte)num6)) : (num3 + ((sbyte)num6)));
                    dst[num2++] = (byte)num6;
                    num -= 1U;
                    array[num4] = (char)num6;
                    num4++;
                    num4 &= 4095;
                }
                else {
                    var num7 = input.ReadByte();
                    var num8 = input.ReadByte();
                    num7 |= (num8 & 240) << 4;
                    num8 &= 15;
                    num8 += 2;
                    var j = num4 - num7;
                    var num9 = num8 + j;
                    if ((num8 + 1) > (num)) throw new ArgumentException("LZSS overflow");
                    while (j <= num9) {
                        var num10 = (int)((byte)array[j & 4095]);
                        num3 = ((!useSignedChecksum) ? (num3 + ((byte)num10)) : (num3 + ((sbyte)num10)));
                        dst[num2++] = (byte)num10;
                        num -= 1U;
                        array[num4] = (char)num10;
                        num4++;
                        num4 &= 4095;
                        j++;
                    }
                }
            }
            var array2 = new byte[4];
            input.Read(array2, 0, 4);
            if (BitConverter.ToInt32(array2, 0) != num3) throw new ArgumentException("Checksum mismatch");
            return (uint)(input.Position - position);
        }

    public static byte[]? Decompress(byte[] compressedData, long targetLength) {

        Stream source = new MemoryStream(compressedData);

        using var reader = new BinaryReader(source, Encoding.UTF8, true);
        using var dest = new MemoryStream();
        using var writer = new BinaryWriter(dest);

        var ctx = new ProcessContext {
            Reader = reader,
            Writer = writer,
            Dest = dest
        };

        var noOfBytes = dest.Position + targetLength;
        while (dest.Position < noOfBytes && source.CanRead) {
            var format = reader.ReadByte();
            for (byte i = 0; i < 8 && dest.Position < noOfBytes && source.Position < source.Length - 2; i++) {
                ctx.Format = format >> i & 0x01;
                ProcessBlock(ctx);
            }
        }
        
        return Validate(ctx) ? dest.ToArray() : null;
    }

    private static IEnumerable<byte> CalculateChecksum(IEnumerable<byte> data) => 
        BitConverter.GetBytes(data.Aggregate<byte, uint>(0, (current, t) => current + t));
    
    private static bool Validate(ProcessContext ctx) {
        const byte intLength = 0;
        var valid = false;            
        var source = ctx.Reader.BaseStream;
        if (source.Length - source.Position < intLength) return valid;
        var crc = ctx.Reader.ReadUInt32();
        valid = crc == ctx.Crc;
        return valid;
    }
    
    private static void ProcessBlock(ProcessContext ctx) {  
        if (ctx.Format == PacketFormatUncompressed) {
            ctx.Write(ctx.Reader.ReadByte());
        } else {
            var pointer = ctx.Reader.ReadInt16();
            var rPosition = ctx.Dest.Position - ((pointer & 0x00FF) + ((pointer & 0xF000) >> 4));                
            var rLength = (byte)(((pointer & 0x0F00) >> 8) + 3);

            if (rPosition + rLength < 0) {
                for (var i = 0; i < rLength; i++) ctx.Write(Space);
            } else {
                while (rPosition < 0) {
                    ctx.Write(Space);
                    rPosition++;
                    rLength--;
                }

                if (rLength <= 0) return;
                var chunkSize = rPosition + rLength > ctx.Dest.Position ? (byte)(ctx.Dest.Position - rPosition) : rLength;
                ctx.SetBuffer(rPosition, chunkSize);

                while (rLength >= chunkSize) {
                    ctx.Write(ctx.Buffer, chunkSize);
                    rLength -= chunkSize;
                }
                for (var j = 0; j < rLength; j++) ctx.Write(ctx.Buffer[j]);
            }
        }
    }
    
    private sealed class Packet {
        private const int DataBlockCount = 8;
        private const int MinPackBytes = 3;
        private const int MaxDataBlockSize = MinPackBytes + 0b1111;
        private const int MaxOffsetForWhitespaces = 0b0000111111111111 - MaxDataBlockSize;
        private int _flagBits;
        
        private readonly List<byte> _content = new();
        private List<byte> _next = new();
        private CompressionBuffer? _compressionBuffer;

        public int Pack(byte[] data, int currPos, CompressionBuffer? buffer) {
            _compressionBuffer = buffer;

            for (var i = 0; i < DataBlockCount && currPos < data.Length; i++) {
                var blockSize = Math.Min(MaxDataBlockSize, data.Length - currPos);
                if (blockSize < MinPackBytes) {
                    currPos += AddUncompressed(i, data, currPos);
                    continue;
                }
                
                currPos += AddCompressed(i, data, currPos, blockSize);
            }

            return currPos;
        }

        public IEnumerable<byte> GetContent() {
            var output = new byte[1 + _content.Count];
            output[0] = BitConverter.GetBytes(_flagBits)[0];

            for (var i = 1; i < output.Length; i++) output[i] = _content[i - 1];

            return output;
        }

        private int AddUncompressed(int blockIndex, IReadOnlyList<byte> data, int currPos) {
            _compressionBuffer?.AddByte(data[currPos]);
            _content.Add(data[currPos]);
            _flagBits += 1 << blockIndex;
            return 1;
        }

        private int AddCompressed(int blockIndex, byte[] data, int currPos, int blockSize) {
            _next = new List<byte>();
            for (var i = 0; i < blockSize; i++) _next.Add(data[currPos + i]);

            var next = _next.ToArray();
            var intersection = _compressionBuffer!.Intersect(next, blockSize);
            var whitespace = currPos < MaxOffsetForWhitespaces 
                ? CompressionBuffer.CheckWhiteSpace(next, blockSize)
                : 0;
            var sequence = _compressionBuffer.CheckSequence(next, blockSize);

            if (intersection.Length < MinPackBytes && whitespace < MinPackBytes && sequence.SourceBytes < MinPackBytes) 
                return AddUncompressed(blockIndex, data, currPos);

            int processed;
            short pointer;

            if (intersection.Length >= whitespace && intersection.Length >= sequence.SourceBytes) {
                pointer = CreatePointer(_compressionBuffer.GetLength() - intersection.Position, intersection.Length);
                processed = intersection.Length;
            }
            else if (whitespace >= intersection.Length && whitespace >= sequence.SourceBytes) {
                pointer = CreatePointer(currPos + whitespace, whitespace);
                processed = whitespace;
            }
            else {
                pointer = CreatePointer(sequence.SequenceBytes, sequence.SourceBytes);
                processed = sequence.SourceBytes;
            }

            _compressionBuffer.AddBytes(data, currPos, processed);
            var tmp = BitConverter.GetBytes(pointer);
            foreach (var t in tmp) _content.Add(t);

            return processed;
        }

        private static short CreatePointer(int offset, int length) {
            var lengthEntry = (short) ((length - MinPackBytes) << 8);//4 bits | 00001111 00000000
            var offsetEntry = (short) (((offset & 0x0F00) << 4) + (offset & 0x00FF));//12 bits | 11110000 11111111

            return (short) (offsetEntry + lengthEntry);
        }
    }
    
    private sealed class CompressionBuffer {
        public struct Intersection {
            public int Position;
            public int Length;
        }

        public struct Sequence {
            public int SourceBytes;
            public int SequenceBytes;
        }

        //4095 ---> 2^12
        private readonly long _size = 0b0000111111111111;
        private readonly List<byte> _content;

        public CompressionBuffer(long size = 0) {
            if (size != 0) _size = size;
            _content = new List<byte>();
        }

        public int GetLength() => _content.Count;

        public void AddBytes(byte[] data, int currPos, int length) {
            if (data == null) throw new ArgumentNullException(nameof(data));
            for (var i = 0; i < length; i++) {
                if (_size < _content.Count + 1) _content.RemoveAt(0);

                _content.Add(data[currPos + i]);
            }
        }

        public void AddByte(byte data) {
            if (_size < _content.Count + 1) _content.RemoveAt(0);

            _content.Add(data);
        }

        public Intersection Intersect(IReadOnlyList<byte> buffer, int length) {
            var intersection = new Intersection {
                Position = -1,
                Length = 0
            };

            if (length == 0 || _content.Count == 0) return intersection;

            var offset = 0;
            while (true) {
                var next = IntersectAtOffset(buffer, length, offset);
                if (next.Position >= 0 && intersection.Length < next.Length) intersection = next;
                if (next.Position < 0 || next.Position > _content.Count - 1) break;

                offset = next.Position + 1;
            }

            return intersection;
        }

        private Intersection IntersectAtOffset(IReadOnlyList<byte> buffer, int bLength, int offset) {
            var position = _content.IndexOf(buffer[0], offset);
            var length = 0;

            if (position >= 0 && position < _content.Count) {
                length++;
                for (int bufIndex = 1, dataIndex = position + 1;
                     bufIndex < bLength && dataIndex < _content.Count;
                     bufIndex++, dataIndex++) {
                    if (_content[dataIndex] != buffer[bufIndex]) break;

                    length++;
                }
            }

            Intersection intersection;
            intersection.Position = position;
            intersection.Length = length;
            return intersection;
        }


        public static int CheckWhiteSpace(IReadOnlyList<byte> buffer, int length) {
            var count = 0;
            for (var i = 0; i < length; i++) {
                if (buffer[i] != 0x20) break;
                count++;
            }

            return count;
        }

        public Sequence CheckSequence(IReadOnlyList<byte> buffer, int length) {
            Sequence result;
            result.SequenceBytes = 0;
            result.SourceBytes = 0;

            var maxSourceBytes = Math.Min(_content.Count, length);
            for (var i = 1; i < maxSourceBytes; i++) {
                var sequence = CheckSequenceImpl(buffer, length, i);
                if (sequence.SourceBytes > result.SourceBytes) result = sequence;
            }

            return result;
        }

        private Sequence CheckSequenceImpl(IReadOnlyList<byte> buffer, int length, int sequenceBytes) {
            var sourceBytes = 0;
            Sequence sequence;

            while (sourceBytes < length) {
                for (var i = _content.Count - sequenceBytes; i < _content.Count && sourceBytes < length; i++) {
                    if (buffer[sourceBytes] != _content[i]) {
                        sequence.SourceBytes = sourceBytes;
                        sequence.SequenceBytes = sequenceBytes;
                        return sequence;
                    }

                    sourceBytes++;
                }
            }

            sequence.SourceBytes = sourceBytes;
            sequence.SequenceBytes = sequenceBytes;
            return sequence;
        }
    }
    
    private sealed class ProcessContext {
        internal BinaryReader Reader = null!;
        internal BinaryWriter Writer = null!;
        internal Stream Dest = null!;
        internal int Format;
        internal readonly byte[] Buffer = new byte[18];
        internal uint Crc;

        private void UpdateCrc(byte data) {
            unchecked {
                Crc += data;
            }
        }

        private void UpdateCrc(IReadOnlyList<byte> chunk, byte chunkSize) {
            unchecked {
                for (byte i = 0; i < chunkSize; i++) Crc += chunk[i];
            }
        }

        internal void Write(byte data) {
            Writer.Write(data);
            UpdateCrc(data);
        }

        internal void Write(byte[] chunk, byte chunkSize) {
            Writer.Write(chunk, 0, chunkSize);
            UpdateCrc(chunk, chunkSize);
        }

        internal void SetBuffer(long offset, byte length) {
            Dest.Seek(offset, SeekOrigin.Begin);
            Dest.Read(Buffer, 0, length);
            Dest.Seek(0, SeekOrigin.End);
        }
    }
}