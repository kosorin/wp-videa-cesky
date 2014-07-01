﻿using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace VideaCesky
{
    public class Subtitles : List<Subtitle>
    {
        public static string srtPattern = @"\d+\r\n(?<start>\S+)\s-->\s(?<end>\S+)\r\n(?<text>(.|[\r\n])+?)\r\n\r\n";

        public async static Task<Subtitles> Download(string uri)
        {
            string srt = "";
            bool isError = false;
            try
            {
                HttpResponse response = await Http.GetAsync(uri);
                srt = response.Response;
            }
            catch (OperationCanceledException)
            {
                // TODO add your cancellation logic
                isError = true;
            }
            catch (Exception)
            {
                isError = true;
            }

            if (isError)
            {
                MessageDialog messageDialog = new MessageDialog("Chyba při stahování titulků!");
                await messageDialog.ShowAsync();
            }
            return Parse(srt);
        }

        public static Subtitles Parse(string srt)
        {
            srt += "\r\n\r\n";
            Subtitles subtitles = new Subtitles();

            var matches = Regex.Matches(srt, srtPattern);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

                Subtitle subtitle = new Subtitle();
                subtitle.Start = TimeSpan.Parse(groups["start"].Value.Replace(',', '.'));
                subtitle.End = TimeSpan.Parse(groups["end"].Value.Replace(',', '.'));
                subtitle.Text = groups["text"].Value;

                subtitles.Add(subtitle);
            }

            return subtitles;
        }
    }
}
