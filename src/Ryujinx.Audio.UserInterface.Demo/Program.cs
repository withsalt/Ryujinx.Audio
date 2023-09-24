using System;
using System.IO;
using System.Linq;
using NAudio.Wave;
using Ryujinx.Audio.Common;
using Ryujinx.Audio.Output;

namespace Ryujinx.Audio.UserInterface.Demo
{
    internal class Program
    {
        static AudioInputConfiguration parameter = new AudioInputConfiguration()
        {
            SampleRate = Constants.TargetSampleRate,
            ChannelCount = 2,  //default is 2
        };

        static void Main(string[] args)
        {
            InitializeAudioOut audio = new InitializeAudioOut();
            audio.InitializeAudioRenderer(parameter, out AudioOutputSystem outSystem);

            var data = File.ReadAllBytes("16k16bit.pcm");
            PlayRowData(outSystem, data, 0, data.Length, new WaveFormat(16000, 1));

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("播放指定wav");
                    using (FileStream stream = new FileStream("2023-09-23-113718_173779.wav", FileMode.Open))
                    {
                        outSystem.Update();
                        PlayWav(outSystem, stream);

                        outSystem.FlushBuffers();
                    }
                }
            }

            Console.WriteLine("Hello, World!");
        }

        static void PlayRowData(AudioOutputSystem outSystem, byte[] source, int offset, int count, WaveFormat format)
        {
            if (source?.Any() != true)
            {
                throw new ArgumentNullException("source", "The source data can not null.");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format", "The format can not null.");
            }
            if (format.SampleRate == parameter.SampleRate && format.Channels == parameter.ChannelCount)
            {
                AudioBuffer audioBuffer = new AudioBuffer()
                {
                    DataPointer = 1,
                    Data = source,
                    DataSize = (ulong)source.Length
                };
                var result = outSystem.AppendBuffer(audioBuffer);
                if (result != ResultCode.Success)
                {
                    throw new Exception("Append Buffer error");
                }
            }
            else
            {
                using RawSourceWaveStream reader = new RawSourceWaveStream(source, 0, source.Length, format);
                using WaveFormatConversionStream converter = new WaveFormatConversionStream(new WaveFormat((int)parameter.SampleRate, (int)parameter.ChannelCount), reader);
                while (true)
                {
                    var buffer = new byte[reader.WaveFormat.AverageBytesPerSecond * 4];
                    int bytesRead = converter.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    AudioBuffer audioBuffer = new AudioBuffer()
                    {
                        DataPointer = 1,
                        Data = buffer,
                        DataSize = (ulong)bytesRead
                    };
                    var result = outSystem.AppendBuffer(audioBuffer);
                    if (result != ResultCode.Success)
                    {
                        //throw new Exception("Append Buffer error");
                    }
                }
            }
        }

        static void PlayWav(AudioOutputSystem outSystem, Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream", "The input stream can not null.");
            }
            WaveFileReader reader = new WaveFileReader(inputStream);
            while (true)
            {
                var buffer = new byte[reader.WaveFormat.AverageBytesPerSecond * 4];
                int bytesRead = reader.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                PlayRowData(outSystem, buffer, 0, bytesRead, reader.WaveFormat);
            }
        }
    }
}