const config = {
  "folderPath": "C:/Users/ericz/Documents/GitHub/uDocuGen2/Assets/Editor/uDocuGen2/Helpers",
  "fileTree": {
    "subDir": [
      {
        "subDir": [
          {
            "subDir": [
              {
                "subDir": [],
                "currentDirectory": "FileReader.cs",
                "filePaths": [
                  "\\FileReader.cs"
                ],
                "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers",
                "fileRepresentation": {
                  "imports": [
                    "System.IO"
                  ],
                  "variables": {
                    "private": [
                      {
                        "attributes": [
                          "private",
                          "string[]"
                        ],
                        "description": "The list where each element is a line of the file ",
                        "accessMod": "private",
                        "varName": "fileText",
                        "dirtyType": "System.String[]",
                        "cleanType": "string[]"
                      },
                      {
                        "attributes": [
                          "private",
                          "int"
                        ],
                        "description": "The line we are reading from in ```fileText``` ",
                        "accessMod": "private",
                        "varName": "index",
                        "dirtyType": "System.Int32",
                        "cleanType": "int"
                      }
                    ]
                  },
                  "inheritance": [
                    "NO INHERITANCE"
                  ],
                  "accessModifier": "public",
                  "className": "FileReader",
                  "description": " <summary> Reads the contents of the file at the specified ```filePath```. <br/> Contains many useful methods that enhances the usability of ```File.ReadAllLines()``` </summary>",
                  "filePath": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers\\FileReader.cs",
                  "nameSpace": "uDocumentGenerator.Helpers",
                  "declaration": "public class FileReader",
                  "functions": {
                    "public": [
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [],
                        "description": "<summary> Read the next line of the file </summary> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "ReadLine"
                      },
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [],
                        "description": "<summary> Read the previous line if it is not an invalid index </summary> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "ReverseLine"
                      },
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [],
                        "description": "<summary> Go back to the beginning of the file </summary> ",
                        "accessModifier": "public",
                        "functionName": "ResetPosition"
                      },
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [],
                        "description": "<summary> Go back to the beginning of the file </summary> ",
                        "accessModifier": "public",
                        "functionName": "Reset"
                      }
                    ]
                  }
                }
              },
              {
                "subDir": [],
                "currentDirectory": "FileTree.cs",
                "filePaths": [
                  "\\FileTree.cs"
                ],
                "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers",
                "fileRepresentation": {
                  "imports": [
                    "System",
                    "System.Collections.Generic",
                    "System.Text.RegularExpressions",
                    "uDocumentGenerator.Generation",
                    "UnityEngine"
                  ],
                  "variables": {
                    "public": [
                      {
                        "attributes": [
                          "public",
                          "List<FileTree>"
                        ],
                        "description": "If this is not a leaf, this will not be null -> provides the recursive structure of FileTree ",
                        "accessMod": "public",
                        "varName": "subDir",
                        "dirtyType": "System.Collections.Generic.List`1[uDocumentGenerator.Helpers.FileTree]",
                        "cleanType": "List<FileTree>"
                      },
                      {
                        "attributes": [
                          "public",
                          "string"
                        ],
                        "description": "The file path to the current directory ",
                        "accessMod": "public",
                        "varName": "currentDirectory",
                        "dirtyType": "System.String",
                        "cleanType": "string"
                      },
                      {
                        "attributes": [
                          "public",
                          "List<string>"
                        ],
                        "description": "The file paths of the files in the subDirs/current level ",
                        "accessMod": "public",
                        "varName": "filePaths",
                        "dirtyType": "System.Collections.Generic.List`1[System.String]",
                        "cleanType": "List<string>"
                      }
                    ],
                    "private": [
                      {
                        "attributes": [
                          "private",
                          "string"
                        ],
                        "description": "The parents & ancestors of this \"file level\" ",
                        "accessMod": "private",
                        "varName": "prevDirectory",
                        "dirtyType": "System.String",
                        "cleanType": "string"
                      },
                      {
                        "attributes": [
                          "private",
                          "FileRepresentation"
                        ],
                        "description": "If this ```FileTree``` is a leaf, it will be populated with a ```FileRepresentation``` ",
                        "accessMod": "private",
                        "varName": "fileRepresentation",
                        "dirtyType": "uDocumentGenerator.Generation.FileRepresentation",
                        "cleanType": "FileRepresentation"
                      }
                    ]
                  },
                  "inheritance": [
                    "NO INHERITANCE"
                  ],
                  "accessModifier": "public",
                  "className": "FileTree",
                  "description": " <summary> The tree is formatted as follows: <ul>     <li>         a file will have no children (a leaf), i.e, ```subDir.Length == 0```     </li>     <li>         a branch will have it's own value and will not end in .cs with non-empty subDir list     </li> </ul> </summary>",
                  "filePath": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers\\FileTree.cs",
                  "nameSpace": "uDocumentGenerator.Helpers",
                  "declaration": "public class FileTree",
                  "functions": {
                    "private": [
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [],
                        "description": "<summary> Builds the FileTree -> contains all the fileReps.   This is a recursive data structure. </summary> ",
                        "accessModifier": "private",
                        "functionName": "BuildTree"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "directories",
                            "Item2": "System.Collections.Generic.List`1[System.String]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "List<string> directories",
                            "Item2": null
                          }
                        ],
                        "modifiers": [],
                        "description": "<summary> Builds a common file path for directories. </summary> <param name=\"directories\"></param> <returns></returns> ",
                        "accessModifier": "private",
                        "functionName": "BuildCommonPath"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "depth",
                            "Item2": "System.Int32",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "int depth",
                            "Item2": null
                          }
                        ],
                        "modifiers": [],
                        "description": "<summary> Returns a formatted string of the file tree </summary> <param name=\"depth\"></param> <returns></returns> ",
                        "accessModifier": "private",
                        "functionName": "StrFormated"
                      }
                    ],
                    "public": [
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [],
                        "description": "<summary> Overrides the default ToString method </summary> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "ToString"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "pathList",
                            "Item2": "System.Collections.Generic.List`1[System.String]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "List<string> pathList",
                            "Item2": null
                          }
                        ],
                        "modifiers": [],
                        "description": "<summary> This will return the FileRepresentation of a file if it exists. Otherwise it will return null. The file path should be split among slashes with the last element being the file.cs </summary> <param name=\"pathList\"></param> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "FindFile"
                      }
                    ]
                  }
                }
              },
              {
                "subDir": [],
                "currentDirectory": "ProjectRepresentation.cs",
                "filePaths": [
                  "\\ProjectRepresentation.cs"
                ],
                "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers",
                "fileRepresentation": {
                  "imports": [
                    "System.Collections.Generic",
                    "System.IO",
                    "System.Linq",
                    "UnityEngine"
                  ],
                  "variables": {
                    "public": [
                      {
                        "attributes": [
                          "public",
                          "string"
                        ],
                        "description": "The path to the specified folder we want to turn into a ```ProjectRepresentation``` ",
                        "accessMod": "public",
                        "varName": "folderPath",
                        "dirtyType": "System.String",
                        "cleanType": "string"
                      },
                      {
                        "attributes": [
                          "public",
                          "FileTree"
                        ],
                        "description": "The generated ```FileTree``` object. **Contains the information that will be displayed on the website** ",
                        "accessMod": "public",
                        "varName": "fileTree",
                        "dirtyType": "uDocumentGenerator.Helpers.FileTree",
                        "cleanType": "FileTree"
                      },
                      {
                        "attributes": [
                          "public",
                          "List<string>"
                        ],
                        "description": "A list of the files in the specified folder ",
                        "accessMod": "public",
                        "varName": "fileList",
                        "dirtyType": "System.Collections.Generic.List`1[System.String]",
                        "cleanType": "List<string>"
                      },
                      {
                        "attributes": [
                          "public",
                          "List<string>"
                        ],
                        "description": "The directories we don't want processed ",
                        "accessMod": "public",
                        "varName": "directoryExclusionsList",
                        "dirtyType": "System.Collections.Generic.List`1[System.String]",
                        "cleanType": "List<string>"
                      }
                    ]
                  },
                  "inheritance": [
                    "NO INHERITANCE"
                  ],
                  "accessModifier": "public",
                  "className": "ProjectRepresentation",
                  "description": " <summary> Provides a representation of a project </summary>",
                  "filePath": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers\\ProjectRepresentation.cs",
                  "nameSpace": "uDocumentGenerator.Helpers",
                  "declaration": "public class ProjectRepresentation",
                  "functions": {
                    "private": [
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "fPath",
                            "Item2": "System.String",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "string fPath",
                            "Item2": null
                          }
                        ],
                        "modifiers": [],
                        "description": "<summary> Recursively generate the flattened list of c# files paths </summary> <param name=\"fPath\"></param> <returns></returns> ",
                        "accessModifier": "private",
                        "functionName": "GenerateFileList"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "fPath",
                            "Item2": "System.String",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "string fPath",
                            "Item2": null
                          }
                        ],
                        "modifiers": [],
                        "description": "<summary> Helper method to find the c# files in a directory </summary> <param name=\"fPath\"></param> <returns></returns> ",
                        "accessModifier": "private",
                        "functionName": "FindcsharpFiles"
                      }
                    ]
                  }
                }
              },
              {
                "subDir": [],
                "currentDirectory": "TextSanitizer.cs",
                "filePaths": [
                  "\\TextSanitizer.cs"
                ],
                "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers",
                "fileRepresentation": {
                  "imports": [
                    "System.Collections.Generic",
                    "UnityEngine",
                    "System.Linq"
                  ],
                  "variables": {
                    "private": [
                      {
                        "attributes": [
                          "private",
                          "static",
                          "readonly",
                          "string"
                        ],
                        "description": "The application path ",
                        "accessMod": "private",
                        "varName": "appPath",
                        "dirtyType": "System.String",
                        "cleanType": "string"
                      }
                    ]
                  },
                  "inheritance": [
                    "NO INHERITANCE"
                  ],
                  "accessModifier": "public",
                  "className": "TextSanitizer",
                  "description": " <summary> Contains static methods for various text/string operations </summary>",
                  "filePath": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2\\Helpers\\TextSanitizer.cs",
                  "nameSpace": "uDocumentGenerator.Helpers",
                  "declaration": "public class TextSanitizer",
                  "functions": {
                    "public": [
                      {
                        "dirtyParamList": [],
                        "cleanParamList": [],
                        "modifiers": [
                          "static"
                        ],
                        "description": "",
                        "accessModifier": "public",
                        "functionName": "get_AppPath"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "path",
                            "Item2": "System.String",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "string path",
                            "Item2": null
                          }
                        ],
                        "modifiers": [
                          "static"
                        ],
                        "description": "<summary> Removes the application data path from the string </summary> <param name=\"path\"></param> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "RemoveApplicationPath"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "stringList",
                            "Item2": "System.Collections.Generic.List`1[System.String]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "List<string> stringList",
                            "Item2": null
                          }
                        ],
                        "modifiers": [
                          "static"
                        ],
                        "description": "<summary> Overload for RemoveApplication path. Takes a list of paths instead of just a path. </summary> <param name=\"stringList\"></param> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "RemoveApplicationPath"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "filePaths",
                            "Item2": "System.Collections.Generic.List`1[System.String]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "List<string> filePaths",
                            "Item2": null
                          }
                        ],
                        "modifiers": [
                          "static"
                        ],
                        "description": "<summary> Reverses the slashes in file paths to keep them consistent </summary> <param name=\"filePaths\"></param> ",
                        "accessModifier": "public",
                        "functionName": "ReverseSlashes"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "target",
                            "Item2": "System.Collections.Generic.List`1[System.String]",
                            "Item3": null
                          },
                          {
                            "Item1": "toRemove",
                            "Item2": "System.Collections.Generic.List`1[System.String]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "List<string> target",
                            "Item2": null
                          },
                          {
                            "Item1": "List<string> toRemove",
                            "Item2": null
                          }
                        ],
                        "modifiers": [
                          "static"
                        ],
                        "description": "",
                        "accessModifier": "public",
                        "functionName": "RemoveCommonDirectory"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "toProcess",
                            "Item2": "System.String",
                            "Item3": null
                          },
                          {
                            "Item1": "removeChars",
                            "Item2": "System.Char[]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "string toProcess",
                            "Item2": null
                          },
                          {
                            "Item1": "char[] removeChars",
                            "Item2": null
                          }
                        ],
                        "modifiers": [
                          "static"
                        ],
                        "description": "<summary> Removes characters specified from a string. Used in cleaning a line from a file. Non-descructive.  </summary> <param name=\"toProcess\"></param> <param name=\"removeChars\"></param> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "RemoveCharacters"
                      },
                      {
                        "dirtyParamList": [
                          {
                            "Item1": "line",
                            "Item2": "System.String",
                            "Item3": null
                          },
                          {
                            "Item1": "searches",
                            "Item2": "System.String[]",
                            "Item3": null
                          }
                        ],
                        "cleanParamList": [
                          {
                            "Item1": "string line",
                            "Item2": null
                          },
                          {
                            "Item1": "string[] searches",
                            "Item2": null
                          }
                        ],
                        "modifiers": [
                          "static"
                        ],
                        "description": "<summary> Finds the comment type from a list of: ```{ \"\", \"//\", \"/*\", \"*\", \"*/\" }```. Returns -1 iff no matches </summary> <param name=\"line\"></param> <param name=\"searches\"></param> <returns></returns> ",
                        "accessModifier": "public",
                        "functionName": "FindCommentType"
                      }
                    ]
                  }
                }
              }
            ],
            "currentDirectory": "Helpers",
            "filePaths": [
              "\\Helpers\\FileReader.cs"
            ],
            "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor\\uDocuGen2",
            "fileRepresentation": null
          }
        ],
        "currentDirectory": "uDocuGen2",
        "filePaths": [
          "\\uDocuGen2\\Helpers\\FileReader.cs"
        ],
        "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets\\Editor",
        "fileRepresentation": null
      }
    ],
    "currentDirectory": "Editor",
    "filePaths": [
      "\\Editor\\uDocuGen2\\Helpers\\FileReader.cs",
      "\\Editor\\uDocuGen2\\Helpers\\FileTree.cs",
      "\\Editor\\uDocuGen2\\Helpers\\ProjectRepresentation.cs",
      "\\Editor\\uDocuGen2\\Helpers\\TextSanitizer.cs"
    ],
    "prevDirectory": "C:\\Users\\ericz\\Documents\\GitHub\\uDocuGen2\\Assets",
    "fileRepresentation": null
  },
  "fileList": [
    "\\Editor\\uDocuGen2\\Helpers\\FileReader.cs",
    "\\Editor\\uDocuGen2\\Helpers\\FileTree.cs",
    "\\Editor\\uDocuGen2\\Helpers\\ProjectRepresentation.cs",
    "\\Editor\\uDocuGen2\\Helpers\\TextSanitizer.cs"
  ],
  "directoryExclusionsList": [
    "\\Tests"
  ]
}