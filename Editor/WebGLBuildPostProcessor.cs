using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UniJS.Editor
{
    public class WebGLBuildPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;
        private const string CDN_TAG = "<script src=\"https://cdn.jsdelivr.net/npm/@pizzapopcorn/unijs/dist/unity.min.js\"></script>";

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL) return;

            string buildPath = report.summary.outputPath;
            string indexPath = Path.Combine(buildPath, "index.html");

            if (!File.Exists(indexPath)) return;

            var settings = UniJSConfig.GetOrCreateSettings();
            string htmlContent = File.ReadAllText(indexPath);
            
            //Importing CDN if configured
            if (settings.includeCDN && !htmlContent.Contains("dist/unity.min.js"))
            {
                htmlContent = htmlContent.Replace("</head>", $"  {CDN_TAG}\n</head>");
            }

            //JS code extraction to separate file
            var match = Regex.Match(htmlContent, @"<script>(.*?)createUnityInstance.*?</script>", RegexOptions.Singleline);
            
            if (match.Success)
            {
                string jsContent = match.Value.Replace("<script>", "").Replace("</script>", "");
                File.WriteAllText(Path.Combine(buildPath, "index.js"), jsContent);
                htmlContent = Regex.Replace(htmlContent, @"<script>.*?</script>", 
                    "<script src=\"index.js\"></script>", RegexOptions.Singleline);
                
                File.WriteAllText(indexPath, htmlContent);
                Debug.Log("<color=#4CAF50><b>UniJs:</b> Build procesado y JS extraído con éxito.</color>");
            }
        }
    }
}