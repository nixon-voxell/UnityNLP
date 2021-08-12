namespace Voxell.NLP.Stem
{
  /// <summary>
  /// CherubNLP Stemmer Factory
  /// In linguistic morphology and information retrieval, 
  /// stemming is the process of reducing inflected (or sometimes derived) words to their word stem, 
  /// base or root form—generally a written word form.
  /// </summary>
  /// <typeparam name="IStem"></typeparam>
  public class StemmerFactory<IStem> where IStem : IStemmer, new()
  {
    private IStem _stemmer;

    private StemOptions _options;

    public StemmerFactory(StemOptions options)
    {
      _options = options;
      _stemmer = new IStem();
    }

    public string Stem(string word) => _stemmer.Stem(word, _options);
  }
}
