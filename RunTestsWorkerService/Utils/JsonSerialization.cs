﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModelsLibrary.Models.AppSpecific;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using Serilog;

namespace RunTestsWorkerService.Utils
{
    /// <summary>
    /// Functions for performing common Json Serialization operations.
    /// <para>Requires the Newtonsoft.Json assembly (Json.Net package in NuGet Gallery) to be referenced in your project.</para>
    /// <para>Only public properties and variables will be serialized.</para>
    /// <para>Use the [JsonIgnore] attribute to ignore specific public properties or variables.</para>
    /// <para>Object to be serialized must have a parameterless constructor.</para>
    /// </summary>
    public static class JsonSerialization
    {
        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto});
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"[JsonSerialization].[WriteToJsonFile] {ex.Message}");
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"[JsonSerialization].[ReadFromJsonFile] {ex.Message}");
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

		internal static string SerializeToJsonModed<T>(T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                jsonResolver.IgnoreProperty(typeof(Test), "Query");
                jsonResolver.IgnoreProperty(typeof(Test), "NativeVerifications");
                jsonResolver.IgnoreProperty(typeof(Test), "ExternalVerifications");
                jsonResolver.IgnoreProperty(typeof(Test), "Retain");
                jsonResolver.IgnoreProperty(typeof(Workflow), "Retain");
                jsonResolver.IgnoreProperty(typeof(Workflow), "StressTest");

                JsonSerializerSettings j = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                j.ContractResolver = jsonResolver;
                return JsonConvert.SerializeObject(objectToWrite, j);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"[JsonSerialization].[SerializeToJsonModed] {ex.Message}");
                throw ex; 
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
	}
}