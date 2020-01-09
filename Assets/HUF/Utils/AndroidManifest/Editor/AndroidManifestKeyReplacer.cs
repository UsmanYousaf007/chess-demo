using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using HUF.Utils.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HUF.Utils.AndroidManifest.Editor
{
    public class AndroidManifestKeyReplacer
    {
        readonly string logPrefix;
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public AndroidManifestKeyReplacer()
        {
            logPrefix = GetType().Name;
        }

        public void CreateFinalManifest(Object target)
        {
            CreateFinalManifest((AndroidManifestKeysConfig) target);    
        }

        public void CreateFinalManifest(AndroidManifestKeysConfig config)
        {
            var xmlDocument = ReadXmlFile(config.AndroidManifestTemplatePath);
            if (xmlDocument == null)
                return;

            var type = config.GetType();
            var fields = type.GetFields(flags);
            var properties = type.GetProperties(flags);
            var fieldsAndProperties = fields.Cast<MemberInfo>().Concat(properties);
            
            foreach (var field in fieldsAndProperties)
            {
                var attribute = field.GetCustomAttribute(typeof(AndroidManifestAttribute), false) as AndroidManifestAttribute;
                if (attribute == null)
                    continue;
                
                var key = field is FieldInfo 
                    ? (string) (field as FieldInfo).GetValue(config) 
                    : (string) (field as PropertyInfo)?.GetValue(config);
                
                var nodes = xmlDocument.GetElementsByTagName(attribute.Tag);
                var matchingNodes = FindNodesWithAttribute(nodes, attribute);

                foreach (var node in matchingNodes)
                {
                    if (key.IsNullOrEmpty())
                    {
                        node.ParentNode?.RemoveChild(node);
                    }
                    else
                    {
                        ReplaceAttributeValue(node, attribute, key);
                    }
                }
            }

            WriteXmlToFile(config.AndroidManifestTemplatePath, xmlDocument);
        }

        void ReplaceAttributeValue(XmlNode node, AndroidManifestAttribute attribute, string key)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name == attribute.Attribute && 
                    attr.Value == attribute.ValueToReplace)
                {
                    attr.Value = key;
                }
            }
        }

        IEnumerable<XmlNode> FindNodesWithAttribute(XmlNodeList nodes, AndroidManifestAttribute attribute)
        {
            var matchingNodes = new List<XmlNode>();
            
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null)
                {
                    continue;
                }
                
                foreach (XmlAttribute nodeAttribute in node.Attributes)
                {
                    if (nodeAttribute.Name == attribute.Attribute &&
                        nodeAttribute.Value == attribute.ValueToReplace)
                    {
                        matchingNodes.Add(node);
                    }
                }
            }

            return matchingNodes;
        }

        XmlDocument ReadXmlFile(string path)
        {
            var xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(path);
            }
            catch (Exception)
            {
                Debug.LogWarning($"[{logPrefix}] Manifest template for {path} not found.");
            }

            return xmlDocument;
        }

        void WriteXmlToFile(string path, XmlDocument xmlDocument)
        {
            var strippedPath = path
                .Split('/')
                .Where(x => !x.Contains(".xml"))
                .ToList();
            strippedPath.Add("AndroidManifest.xml");
            var finalPath = Path.Combine(strippedPath.ToArray());
            xmlDocument.Save(finalPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log($"[{logPrefix}] Created new manifest file at {finalPath}");
        }
    }
}