using NAudio.Wave;
using NAudio.Lame;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using MediaToolkit.Model;
using MediaToolkit;
using MediaToolkit.Options;

namespace pocAudio.Controllers
{
    public class AudioController : ApiController
    {
        // GET api/audio

        public HttpResponseMessage Get()
        {
            string inputFile = @"C:\dev\Projetos\AudioBunker\Audios\teste.wav";

            byte[] waveBytes = File.ReadAllBytes(inputFile);
            //byte[] mp3Bytes = ConvertWavToMp3(waveBytes);
            byte[] mp3Bytes2 = ConvertWavToMp3NAudio(waveBytes);

            // Retorna o MP3 como resposta
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(mp3Bytes2)
            };
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "teste.mp3"
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");

            return response;
        }

        public byte[] ConvertWavToMp3(byte[] waveBytes)
        {
            // Write the WAV bytes to a temporary file
            string tempWavFile = Path.GetTempFileName();
            File.WriteAllBytes(tempWavFile, waveBytes);

            // Create a temporary file for the MP3
            string tempMp3File = Path.GetTempFileName();

            var inputFile = new MediaFile { Filename = tempWavFile };
            var outputFile = new MediaFile { Filename = tempMp3File };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                var options = new ConversionOptions { AudioSampleRate = AudioSampleRate.Hz44100 };
                engine.Convert(inputFile, outputFile, options);
            }

            // Read the MP3 bytes from the temporary file
            byte[] mp3Bytes = File.ReadAllBytes(tempMp3File);

            // Delete the temporary files
            File.Delete(tempWavFile);
            File.Delete(tempMp3File);

            return mp3Bytes;
        }


        public byte[] ConvertWavToMp3NAudio(byte[] waveBytes)
        {
            //using (var mp3Stream = new MemoryStream())
            //{
            //    using (var reader = new WaveFileReader(new MemoryStream(waveBytes)))
            //    {
            //        using (var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
            //        {
            //            using (var writer = new LameMP3FileWriter(mp3Stream, pcmStream.WaveFormat, LAMEPreset.ABR_128))
            //            {
            //                pcmStream.CopyTo(writer);
            //            }
            //        }
            //    }

            //    return mp3Stream.ToArray();
            //}
            using (var retMs = new MemoryStream())
            {
                using (var ms = new MemoryStream(waveBytes))
                {
                    using (var rdr = new WaveFileReader(ms))
                    {
                        var pcmStream = WaveFormatConversionStream.CreatePcmStream(rdr);

                        using (var wtr = new LameMP3FileWriter(retMs, pcmStream.WaveFormat, LAMEPreset.ABR_128))
                        {
                            pcmStream.CopyTo(wtr);
                        }
                    }
                }

                return retMs.ToArray();
            }
        }
    }
}
