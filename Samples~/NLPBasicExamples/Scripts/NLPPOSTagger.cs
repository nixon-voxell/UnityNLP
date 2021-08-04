using UnityEngine;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Tokenize;
using Voxell.Inspector;
using Voxell;

public class NLPPOSTagger : MonoBehaviour
{
  [StreamingAssetFilePath]
  public string tokenizerModel;
  [StreamingAssetFilePath]
  public string posTaggerModel;
  [StreamingAssetFilePath]
  public string tagDict;
  [TextArea(1, 5)] public string sentence;
  public string[] tokens;
  public string[] posTags;

  private EnglishMaximumEntropyTokenizer tokenizer;
  private EnglishMaximumEntropyPosTagger posTagger;

  [Button]
  public void Tag()
  {
    // link to POS tags meanings: https://www.ling.upenn.edu/courses/Fall_2003/ling001/penn_treebank_pos.html
    tokenizer = new EnglishMaximumEntropyTokenizer(FileUtil.GetStreamingAssetFilePath(tokenizerModel));
    posTagger = new EnglishMaximumEntropyPosTagger(
      FileUtil.GetStreamingAssetFilePath(posTaggerModel),
      FileUtil.GetStreamingAssetFilePath(tagDict));

    tokens = tokenizer.Tokenize(sentence);
    posTags = posTagger.Tag(tokens);
  }
}
