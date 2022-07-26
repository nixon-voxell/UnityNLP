using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Voxell;
using Voxell.Inspector;
using Voxell.NLP;
using Voxell.NLP.Classifier;
using Voxell.NLP.Tokenize;
using Voxell.NLP.PosTagger;
using Voxell.NLP.Stem;

public class NLPNaiveBayesClassifier : MonoBehaviour
{
  public ClassifyOptions classifyOptions;
  [StreamingAssetFilePath] public string tokenizerModel;
  [StreamingAssetFilePath] public string posTaggerModel;
  [StreamingAssetFilePath] public string tagDict;

  public TextAsset dataset;
  public List<Sentence> sentences;
  public List<string> vocabs;
  public string sentenceToClassify;
  [InspectOnly] public string classifiedLabel;
  [InspectOnly] public double highestConfidence;

  private NaiveBayesClassifier _classifier;
  private EnglishMaximumEntropyTokenizer _tokenizer;
  private EnglishMaximumEntropyPosTagger _posTagger;
  private RegexStemmer _stemmer;

  public void InitializeData()
  {
    // reset data
    sentences.Clear();
    classifyOptions.labels.Clear();

    // create _tokenizer, pos tagger, and _stemmer
    _tokenizer = new EnglishMaximumEntropyTokenizer(FileUtilx.GetStreamingAssetFilePath(tokenizerModel));
    _posTagger = new EnglishMaximumEntropyPosTagger(
      FileUtilx.GetStreamingAssetFilePath(posTaggerModel),
      FileUtilx.GetStreamingAssetFilePath(tagDict));
    _stemmer = new RegexStemmer();
    _stemmer.CreatePattern();

    // generate data
    JObject data = JsonConvert.DeserializeObject<JObject>(dataset.text);
    JToken intents = data["intents"];

    foreach (JToken intent in intents)
    {
      string label = (string)intent["intent"];
      classifyOptions.AddLabel(label);
      // convert each sentences into a Sentence class and add it into the list
      foreach (JToken text in intent["text"])
        sentences.Add(new Sentence(
          ((string)text).ToLower(),
          label,
          _tokenizer, _posTagger, _stemmer
        ));
    }
  }

  [Button]
  public void TrainClassifier()
  {
    InitializeData();
    // train and save the model
    _classifier = new NaiveBayesClassifier();
    _classifier.Train(sentences, classifyOptions);
    _classifier.SaveModel(classifyOptions);
  }

  [Button]
  public void Classify()
  {
    if (_tokenizer == null)
    {
      // recreate _tokenizer, pos tagger, and _stemmer if editor is being refreshed
      _tokenizer = new EnglishMaximumEntropyTokenizer(FileUtilx.GetStreamingAssetFilePath(tokenizerModel));
      _posTagger = new EnglishMaximumEntropyPosTagger(
        FileUtilx.GetStreamingAssetFilePath(posTaggerModel),
        FileUtilx.GetStreamingAssetFilePath(tagDict));
      _stemmer = new RegexStemmer();
      _stemmer.CreatePattern();
    }

    // convert string sentence to Sentence class
    Sentence sent = new Sentence(sentenceToClassify.ToLower(), "", _tokenizer, _posTagger, _stemmer);
    _classifier = new NaiveBayesClassifier();
    _classifier.LoadModel(classifyOptions);

    // take a look at all the vocabs that the _classifier stored
    vocabs = _classifier.words;
    List<Tuple<string, double>> result = _classifier.Classify(sent, classifyOptions);

    classifiedLabel = "";
    highestConfidence = 0.0;
    for (int r=0; r < result.Count; r++)
    {
      if (result[r].Item2 > highestConfidence)
      {
        classifiedLabel = result[r].Item1;
        highestConfidence = result[r].Item2;
      }
    }
  }
}
