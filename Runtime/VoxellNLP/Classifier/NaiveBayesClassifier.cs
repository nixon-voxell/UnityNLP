﻿/*
 * CherubNLP Library
 * Copyright (C) 2018 Haiping Chen
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Bigtree.Algorithm.Bayes;
using Bigtree.Algorithm.Features;
using Bigtree.Algorithm.Statistics;
using Voxell.NLP.Featuring;
using Voxell.NLP.Txt2Vec;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Voxell.NLP.Classifier
{
  /// <summary>
  /// This is a simple (naive) classification method based on Bayes rule. 
  /// It relies on a very simple representation of the document (called the bag of words representation)
  /// This technique works well for topic classification; 
  /// say we have a set of academic papers, and we want to classify them into different topics (computer science, biology, mathematics).
  /// Naive Bayes is best for Less training data
  /// </summary>
  public class NaiveBayesClassifier : IClassifier
  {
    private List<Probability> labelDist;
    private MultinomiaNaiveBayes nb = new MultinomiaNaiveBayes();
    private Dictionary<string, double> condProbDictionary = new Dictionary<string, double>();

    public List<string> words;
    private double[] features = new double[] { 0, 1 };

    public void Train(List<Sentence> sentences, ClassifyOptions options)
    {
      var tfidf = new TfidfFeatureExtractor();
      tfidf.Dimension = options.dimension;
      tfidf.Sentences = sentences;
      tfidf.CalBasedOnCategory();

      var encoder = new OneHotEncoder();
      encoder.Sentences = sentences;
      encoder.words = tfidf.Keywords();
      words = encoder.EncodeAll();

      List<Tuple<string, double[]>> featureSets = sentences.Select(x => new Tuple<string, double[]>(x.label, x.vector)).ToList();

      labelDist = featureSets.GroupBy(x => x.Item1)
        .Select(x => new Probability
        {
          Value = x.Key,
          Freq = x.Count()
        })
        .OrderBy(x => x.Value)
        .ToList();

      nb.LabelDist = labelDist;
      nb.FeatureSet = featureSets;

      // calculate prior prob
      labelDist.ForEach(l => l.Prob = nb.CalPriorProb(l.Value));

      // calculate posterior prob
      // loop features
      var featureCount = nb.FeatureSet[0].Item2.Length;

      labelDist.ForEach(label =>
      {
        for (int x = 0; x < featureCount; x++)
        {
          for (int v = 0; v < features.Length; v++)
          {
            string key = $"{label.Value} f{x} {features[v]}";
            condProbDictionary[key] = nb.CalCondProb(x, label.Value, features[v]);
          }
        }
      });
    }

    public List<Tuple<string, double>> Classify(Sentence sentence, ClassifyOptions options)
    {
      var encoder = new OneHotEncoder();
      encoder.words = words;
      encoder.Encode(sentence);

      var results = new List<Tuple<string, double>>();

      // calculate prop
      labelDist.ForEach(lf =>
      {
        var prob = nb.CalPosteriorProb(lf.Value, sentence.vector, lf.Prob, condProbDictionary);
        results.Add(new Tuple<string, double>(lf.Value, prob));
      });

      /*Parallel.ForEach(labelDist, (lf) =>
      {
        nb.Y = lf.Value;
        lf.Prob = nb.PosteriorProb();
      });*/

      double total = results.Select(x => x.Item2).Sum();
      return results.Select(x => new Tuple<string, double>(x.Item1, x.Item2 / total)).ToList();
    }

    public void SaveModel(ClassifyOptions options)
    {
      // save the model
      MultinomiaNaiveBayesModel model = new MultinomiaNaiveBayesModel
      {
        LabelDist = labelDist,
        CondProbDictionary = condProbDictionary,
        Values = words
      };

      //save the file
      using (BinaryWriter bw = new BinaryWriter(new FileStream(options.GetModelFilePath(), FileMode.Create)))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
        bw.Write(bytes);
      }
    }

    public void LoadModel(ClassifyOptions options)
    {
      string json = String.Empty;

      //read the file
      using (var br = new BinaryReader(new FileStream(options.GetModelFilePath(), FileMode.Open)))
      {
        byte[] bytes = br.ReadBytes((int)br.BaseStream.Length);
        json = Encoding.UTF8.GetString(bytes);
      }

      MultinomiaNaiveBayesModel model = JsonConvert.DeserializeObject<MultinomiaNaiveBayesModel>(json);

      labelDist = model.LabelDist;
      condProbDictionary = model.CondProbDictionary;
      words = model.Values;
    }
  }

  public class FeaturesWithLabel
  {
    public List<Feature> Features { get; set; }
    public string label { get; set; }
    public FeaturesWithLabel()
    {
      this.Features = new List<Feature>();
    }
  }
}
