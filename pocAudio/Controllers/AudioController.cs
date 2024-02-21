using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NAudio.Lame;
using NAudio.Wave;

namespace pocAudio.Controllers
{
    public class AudioController : ApiController
    {
        // GET api/audio
        public HttpResponseMessage Get()
        {
            string inputFile = @"C:\dev\Projetos\AudioBunker\Audios\teste.wav";

            byte[] waveBytes = File.ReadAllBytes(inputFile);
            byte[] mp3Bytes = ConvertWavBytesToMp3(waveBytes);

            // Retorna o MP3 como resposta
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(mp3Bytes)
            };
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "teste.mp3"
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");

            return response;
        }

        static byte[] ConvertWavBytesToMp3(byte[] wavBytes)
        {
            using (var inputStream = new MemoryStream(wavBytes))
            {
                using (var reader = new WaveFileReader(inputStream))
                {
                    // Converter para PCM (16 bits, 44.1kHz, estéreo)
                    var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);

                    using (var outputStream = new MemoryStream())
                    {
                        // Converter para MP3
                        using (var writer = new LameMP3FileWriter(outputStream, pcmStream.WaveFormat, LAMEPreset.ABR_128))
                        {
                            pcmStream.CopyTo(writer);
                        }
                        return outputStream.ToArray();
                    }
                }
            }
        }
    }
}
