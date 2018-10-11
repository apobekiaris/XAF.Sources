﻿using System;
using System.IO;
using System.Reflection;

namespace PocketXaf.SourcePackages.StreamExtensions{
    public static class StreamExtensions{
        public static void SaveToFile(this Stream stream, string filePath) {
            var directory = Path.GetDirectoryName(filePath) + "";
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            using (var fileStream = File.OpenWrite(filePath)) {
                stream.CopyTo(fileStream);
            }
        }

        public static void WriteResourceToFile(this Type type, string resourceName,string filePath) {
            type.Assembly.GetManifestResourceStream(type,resourceName).SaveToFile(filePath);
        }

        public static string GetResourceString(this Assembly assembly, string name) {
            return assembly.GetManifestResourceStream(name).ReadToEndAsString();
        }

        public static string GetResourceString(this Type type, string name) {
            return type.Assembly.GetManifestResourceStream(type, name).ReadToEndAsString();
        }

        public static string GetDxScriptFromResource(this Type type, string name) {
            return type.Assembly.GetManifestResourceStream(type, name).ReadToEndAsString();
        }

        public static byte[] ReadFully(this Stream input) {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream()) {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string ReadToEndAsString(this Stream stream) {
            using (var streamReader = new StreamReader(stream)) {
                return streamReader.ReadToEnd();

            }
        }
        public static void ReadExactly(Stream input, byte[] buffer, int bytesToRead){
            var index = 0;
            while (index < bytesToRead){
                var read = input.Read(buffer, index, bytesToRead - index);
                if (read == 0)
                    throw new EndOfStreamException
                        ($"End of stream reached with {bytesToRead - index} byte{(bytesToRead - index == 1 ? "s" : "")} left to read.");
                index += read;
            }
        }
    }
}