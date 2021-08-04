using UnityEngine;
using OpenNLP.Tools.NameFind;
using Voxell.Inspector;
using Voxell;

public class NLPNamedEntityRecognition : MonoBehaviour
{
  [StreamingAssetFolderPath]
  public string nameFinderModel;
  [TextArea(1, 5)] public string sentence;
  public string[] models = new string[]
  { "date", "location", "money", "organization", "percentage", "person", "time" };
  [TextArea(1, 5), InspectOnly] public string ner;

  private EnglishNameFinder nameFinder;

  [Button]
  public void Recognize()
  {
    nameFinder = new EnglishNameFinder(FileUtil.GetStreamingAssetFilePath(nameFinderModel));
    ner = nameFinder.GetNames(models, sentence);
  }
}
