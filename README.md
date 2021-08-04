# Unity NLP

This package is still under development. [OpenNLP](https://archive.codeplex.com/?p=sharpnlp) is used as the backend of this package.

## Installation

1. Clone this repository into your Packages folder.
2. Download all [essential models](https://drive.google.com/file/d/19bD2h0LBIArczYtQMHuoqdNRuUZrWdOX/view?usp=sharing) and import them into the project.
3. Place the models in the StreamingAssets folder.
4. And you are ready to go!

## OpenNLP

OpenNLP is a collection of natural language processing tools written in C#. Currently it provides the following NLP tools:
- sentence splitter
- tokenizer
- part-of-speech tagger
- chunker (used to "find non-recursive syntactic annotations such as noun phrase chunks")
- parser
- name finder
- coreference tool
- interface to the WordNet lexical database

*Make a feature request in the issues tab if you think there is something missing or if you have new ideas!*

# Examples

The examples below utilizes the [UnityUtil](https://github.com/voxell-tech/UnityUtil) package. This package is not dependent on UnityUtil, but if you want to follow along the examples and import the samples, it is recommended to install UnityUtil.

## Tokenize

```cs
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

  [Button]
  public void Tokenize()
  {
    EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer(FileUtil.GetStreamingAssetFilePath(tokenizerModel));
    tokens = tokenizer.Tokenize(sentence);
  }
}
```

![Tokenize](Pictures~/TokenizerExample.png)

## Support the project!

<a href="https://www.patreon.com/voxelltech" target="_blank">
  <img src="https://teaprincesschronicles.files.wordpress.com/2020/03/support-me-on-patreon.png" alt="patreon" width="200px" height="56px"/>
</a>

<a href ="https://ko-fi.com/voxelltech" target="_blank">
  <img src="https://uploads-ssl.webflow.com/5c14e387dab576fe667689cf/5cbed8a4cf61eceb26012821_SupportMe_red.png" alt="kofi" width="200px" height="40px"/>
</a>

## Join the community!

<a href ="https://discord.gg/WDBnuNH" target="_blank">
  <img src="https://gist.githubusercontent.com/nixon-voxell/e7ba303906080ffdf65b106f684801b5/raw/65b0338d5f4e82f700d3c9f14ec9fc62f3fd278e/JoinVXDiscord.svg" alt="discord" width="200px" height="200px"/>
</a>


## License

This repository as a whole is licensed under the GNU Public License, Version 3. Individual files may have a different, but compatible license.

See [license file](./LICENSE) for details.
