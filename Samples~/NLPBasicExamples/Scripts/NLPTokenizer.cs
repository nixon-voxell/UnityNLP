using UnityEngine;
using OpenNLP.Tools.Tokenize;
using Voxell.Inspector;
using Voxell;

public class NLPTokenizer : MonoBehaviour
{
  [StreamingAssetFilePath]
  public string tokenizerModel;
  [TextArea(1, 5)] public string sentence;
  public string[] tokens;

  private EnglishMaximumEntropyTokenizer tokenizer;

  [Button]
  public void Tokenize()
  {
    tokenizer = new EnglishMaximumEntropyTokenizer(FileUtil.GetStreamingAssetFilePath(tokenizerModel));
    tokens = tokenizer.Tokenize(sentence);
  }
}
