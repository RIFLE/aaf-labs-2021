using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


namespace PrefixTrieLibrary
{
    public class Node
    {
        private Node anotherBranch;
        private Node childBranch;
        private Node parentBranch;
        private string value;
        private List<int> wordsInNodeIndexed; //It is needed to make a tree look
                                              //more "simple"; example: if u add words
                                              //ad, advert, advertisment, then
                                              //a corresponding Node will look like ad*vert*isement*  
        public enum CompletionStatus
        {
            NEW_ADDED = 1,
            ALREADY_EXISTS = 2,
            NULL = 0,
            EMPTY_TREE = -1,
            NULL_OR_EMPTY_PARAM = -2,
            NO_MATCH_FOUND = -3,
            INVALID_PARAMS = -4
        }

        public Node()
        {
            anotherBranch = childBranch = parentBranch = null;
            value = "";
            wordsInNodeIndexed = new List<int>();
        }

        private void SetValue(string valueContainer)
        {
            this.value = valueContainer;
        }

        public string GetValue()
        {
            return this.value;
        }

        private void SetParentNode(Node parentNodeContainer)
        {
            this.parentBranch = parentNodeContainer;
        }

        public Node GetParentNode()
        {
            return this.parentBranch;
        }

        public string GetTreeAsString()
        {
            StringBuilder buffer = new StringBuilder(50);
            PrintTree(ref buffer, "", "");
            return buffer.ToString();
        }

        private void PrintTree(ref StringBuilder buffer, string prefix, string childrenPrefix)
        {
            buffer.Append(prefix);
            if (this.wordsInNodeIndexed.Any())
            {
                StringBuilder valueWithIndexedWords = new StringBuilder(value);
                wordsInNodeIndexed.Sort();
                for (int itCount = 0; itCount < wordsInNodeIndexed.Count(); itCount++)
                {
                    valueWithIndexedWords.Insert(wordsInNodeIndexed[itCount] + itCount, "*");
                }
                buffer.Append(valueWithIndexedWords.ToString());
            }
            else
            {
                buffer.Append(value);
            }
            buffer.Append('\n');

            Node itNode;
            if (!(this.childBranch == null)) itNode = this.childBranch;
            else return;

            Node nextNode = itNode;
            while (!(nextNode == null))
            {
                if (!(nextNode.anotherBranch == null)) //changed
                {
                    nextNode.PrintTree(ref buffer, childrenPrefix + "├── ", childrenPrefix + "│   ");
                }
                else
                {
                    nextNode.PrintTree(ref buffer, childrenPrefix + "└── ", childrenPrefix + "    ");
                }
                nextNode = nextNode.anotherBranch;
            }
        }

        public CompletionStatus GetMatchesList(ref List<string> wordsList, string pattern1, string pattern2 = "")
        {
            if (this.childBranch == null)
            {
                return CompletionStatus.EMPTY_TREE;
            }
            else if (wordsList == null)
            {
                wordsList = new List<string>() { };
            }
            else if (wordsList.Any())
            {
                wordsList.Clear();
            }

            if (pattern2 == "")
            {
                if (pattern1 == null || pattern1 == "")
                {
                    return CompletionStatus.NULL_OR_EMPTY_PARAM;
                }
                else
                {
                    string @regexPattern = "^" + (@pattern1).Replace("?", @"(\w|\s|\t)").Replace("*", @"(\w|\s|\t){0,}") + "$";
                    var matchFinder = new System.Text.RegularExpressions.Regex(@regexPattern);
                    this.Search(ref wordsList, ref matchFinder, ref @pattern1);
                }
            }
            else if (pattern2 != "" && pattern2.StartsWith(pattern1))
            {
                //if(this.Contains(pattern1) && this.Contains(pattern2))    //only if we want to check if there are these exact examples
                //in the tree to find something exactly between existing nodes;
                //However, we yet can drop it and start search as how it is checked so far
                this.Search(ref wordsList, ref pattern1, ref pattern2);
            }
            else if (!pattern2.StartsWith(pattern1))
            {
                return CompletionStatus.INVALID_PARAMS;
            }
            if (wordsList.Any()) return CompletionStatus.NEW_ADDED;
            else return CompletionStatus.NO_MATCH_FOUND;
        }

        private void Search(ref List<string> wordsList,
                            ref System.Text.RegularExpressions.Regex matchFinder,
                            ref string pattern,
                            string followingLevelPattern = "")   //matches for single pattern
        {

            if (this.wordsInNodeIndexed.Any())
            {
                string exPattern = String.Empty;
                foreach (int word in wordsInNodeIndexed)
                {
                    if (matchFinder.IsMatch(exPattern = followingLevelPattern + this.value.Substring(0, word)))
                    {
                        wordsList.Add(exPattern);
                    }
                }
            }

            followingLevelPattern += this.value;

            if (this.childBranch == null)
            {
                if (matchFinder.IsMatch(followingLevelPattern))
                {
                    wordsList.Add(followingLevelPattern);
                }
            }

            Node itNode;
            if (!(this.childBranch == null)) itNode = this.childBranch;
            else return;

            while (!(itNode == null))
            {
                itNode.Search(ref wordsList, ref matchFinder, ref pattern, followingLevelPattern);
                itNode = itNode.anotherBranch;
            }
        }

        private void Search(ref List<string> wordsList,
                            ref string begPattern,
                            ref string endPattern,
                            string followingLevelPattern = "")           //overload for matches between two patterns
        {
            if (followingLevelPattern.Length > endPattern.Length)
            {
                return;
            }
            else if (followingLevelPattern.Length + this.value.Length >= begPattern.Length)
            {
                if (this.wordsInNodeIndexed.Any())
                {
                    string exPattern = String.Empty;
                    foreach (int word in wordsInNodeIndexed)
                    {
                        exPattern = followingLevelPattern + this.value.Substring(0, word);
                        if (exPattern.StartsWith(begPattern) && endPattern.StartsWith(exPattern))
                        {
                            wordsList.Add(exPattern);
                        }
                    }
                }

                followingLevelPattern += this.value;

                if (this.childBranch == null)
                {
                    if (followingLevelPattern.StartsWith(begPattern) && endPattern.StartsWith(followingLevelPattern))
                    {
                        wordsList.Add(followingLevelPattern);
                    }
                }
            }
            else
            {
                followingLevelPattern += this.value;
            }

            Node itNode;
            if (!(this.childBranch == null)) itNode = this.childBranch;
            else return;

            while (!(itNode == null))
            {
                itNode.Search(ref wordsList, ref begPattern, ref endPattern, followingLevelPattern);

                itNode = itNode.anotherBranch;
            }
        }

        public bool Contains(string patternToCheck, string followingLevelPattern = "")
        {
            Node thisNode = this;
            if (this.value == "")
            {
                if (this.childBranch == null)
                {
                    return false;
                }
                thisNode = this.childBranch;
            }

            string currentPattern = followingLevelPattern;
            while (thisNode != null)
            {
                if (patternToCheck.StartsWith(currentPattern + thisNode.value[0])) //if matches a single char ->
                {
                    currentPattern += thisNode.value;
                    if (currentPattern.StartsWith(patternToCheck))
                    {
                        int checkIndex = currentPattern.Length - thisNode.value.Length;
                        foreach (int word in thisNode.wordsInNodeIndexed)
                        {
                            if ((checkIndex + word) == patternToCheck.Length) return true;
                        }
                        if ((currentPattern == patternToCheck) && (thisNode.childBranch == null))
                        {
                            return true;
                        }
                    }
                    else if (patternToCheck.StartsWith(currentPattern))
                    {
                        Node iteratingNode = thisNode.childBranch;
                        if (iteratingNode == null)
                        {
                            return false;
                        }

                        do
                        {
                            if (patternToCheck.StartsWith(currentPattern + iteratingNode.value[0]))
                            {
                                return iteratingNode.Contains(patternToCheck, currentPattern);
                            }
                            iteratingNode = iteratingNode.anotherBranch;
                        }
                        while (iteratingNode != null); //While there exists another child

                        if (iteratingNode == null)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (thisNode.anotherBranch != null) thisNode = thisNode.anotherBranch;
                    else break;
                }
            }
            return false;
        }

        public CompletionStatus AddNode(string patternToAdd, string followingLevelPattern = "") //brainfuck
        {
            Node thisNode = this; //It is needed to operate within this
                                  //method in a more sophisticated way.
            if (this.value == "")
            {
                if (this.childBranch == null)
                {
                    this.childBranch = new Node();
                    this.childBranch.SetValue(patternToAdd);
                    this.childBranch.SetParentNode(this);
                    return CompletionStatus.NEW_ADDED;
                }
                thisNode = this.childBranch;
            }

            string currentPattern = followingLevelPattern;
            while (thisNode != null)
            {
                if (patternToAdd.StartsWith(currentPattern + thisNode.value[0])) //if matches a single char ->
                {                                                                //its place is in this branch
                    currentPattern += thisNode.value;
                    if (currentPattern.StartsWith(patternToAdd)) //Checking if pattern is itself
                    {                                            //a prefix match for current node.
                        //evaluate position of a word to add it into a list of existing words
                        int rest = thisNode.value.Length - (currentPattern.Length - patternToAdd.Length);
                        if (thisNode.childBranch == null && rest == thisNode.value.Length)
                        {
                            return CompletionStatus.ALREADY_EXISTS;
                        }
                        else if (!thisNode.wordsInNodeIndexed.Contains(rest))
                        {
                            thisNode.wordsInNodeIndexed.Add(rest);
                            return CompletionStatus.NEW_ADDED;
                        }
                        return CompletionStatus.ALREADY_EXISTS;
                    }
                    else if (patternToAdd.StartsWith(currentPattern)) /*exactly starts with pattern*/
                    {                                                 //example: ADVERTisement:ADVERT
                        Node iteratingNode = thisNode.childBranch; //needed as an iterator
                        if (iteratingNode == null)
                        {
                            //No children were added yet; 
                            //adding pattern piece to thisNode.
                            //Hope this works...
                            patternToAdd = patternToAdd.Substring(currentPattern.Length,
                                                                   patternToAdd.Length - currentPattern.Length);
                            thisNode.wordsInNodeIndexed.Add(thisNode.value.Length);
                            thisNode.SetValue(thisNode.value + patternToAdd);
                            return CompletionStatus.NEW_ADDED;
                        }

                        do //If there is any number of children, except zero - we seek among
                           //each for a first char match -> then calling method recursively.
                        {
                            if (patternToAdd.StartsWith(currentPattern + iteratingNode.value[0]))
                            {   //There exists a child with a match,
                                //now must be a descent down into a child branch, exact beginning
                                return iteratingNode.AddNode(patternToAdd, currentPattern);
                                //other children are not needed to be looked through
                            }
                            iteratingNode = iteratingNode.anotherBranch ?? iteratingNode;
                        }
                        while (!(iteratingNode.childBranch == null)); //While there exists another child

                        if (iteratingNode.anotherBranch == null)
                        {   //If yet no branch match found
                            //we delete first currentPattern.Length characters from patternToAdd.
                            //Example: currentPattern = "Good"
                            //         patternToAdd =   "Good morning" => patternToAdd = " morning"

                            patternToAdd = patternToAdd.Substring(currentPattern.Length,
                                                                   patternToAdd.Length - currentPattern.Length);
                            iteratingNode.anotherBranch = new Node();
                            iteratingNode.anotherBranch.SetValue(patternToAdd);
                            iteratingNode.anotherBranch.SetParentNode(iteratingNode);
                            //all fields are set automatically to null
                            //parentSet needs to be everywhere (however, it is a non-inheritable prefix)
                            return CompletionStatus.NEW_ADDED;
                        }
                    }
                    else
                    {
                        string normalPrefixPattern = "";

                        //Algorithm optimizer: get and save position of a string if it is
                        //too long for a current node to add as a normal prefix
                        //Example: pattern_to_add = "There is a line to add, very long"; 
                        //         current_pattern= "There is a line here"
                        //     =>  Normal_prefix_p= "There is a line to a"
                        //Further checks are unnecessarry.

                        normalPrefixPattern = (patternToAdd.Length > currentPattern.Length) ?
                                            patternToAdd.Remove(currentPattern.Length)
                                          : patternToAdd;

                        for (string str = normalPrefixPattern; str.Length > 0; str = str.Remove(str.Length - 1))
                        {
                            if (currentPattern.StartsWith(str))
                            {
                                //str = pattern that is match for currentPattern
                                //mismatchedRest = rest that is gonna be added
                                string mismatchedRest = patternToAdd.Substring(str.Length, patternToAdd.Length - str.Length);

                                Node objectForNewPattern = new Node();
                                objectForNewPattern.SetValue(mismatchedRest);                           //First setNode

                                Node objectForCommonPattern = new Node();
                                int loopCount = normalPrefixPattern.Length - str.Length;
                                int commonPatternLength = normalPrefixPattern.Length - loopCount
                                                        - (currentPattern.Length - thisNode.value.Length);
                                if (commonPatternLength <= 0)
                                {
                                    throw new ApplicationException(message: "Unpredictable behaviour: logical error_2.");
                                }
                                string commonPattern = thisNode.value.Remove(commonPatternLength);

                                objectForCommonPattern.SetValue(commonPattern);                         //Second setNode

                                //Some part of a string that didn't match for pattern
                                //Example: currentPattern ab|cdex
                                //         patternToAdd = abcdf
                                //         leftover = ex

                                string leftover = thisNode.value.Substring(commonPatternLength,
                                                                           thisNode.value.Length - commonPatternLength);
                                thisNode.SetValue(leftover);                                            //Third setNode

                                if (thisNode.wordsInNodeIndexed.Any()) //If there is any list member -> reevaluate and reset
                                {
                                    objectForCommonPattern.wordsInNodeIndexed = thisNode.wordsInNodeIndexed
                                                                                .Where(word => (word <= commonPatternLength)).ToList();
                                    thisNode.wordsInNodeIndexed.RemoveAll(word => (word <= commonPatternLength));
                                    thisNode.wordsInNodeIndexed = thisNode.wordsInNodeIndexed
                                                                  .Select(word => { word -= commonPatternLength; return word; }).ToList();
                                }

                                if (thisNode.parentBranch.childBranch == thisNode) //thisNode is childBranch of a parentNode
                                {
                                    thisNode.parentBranch.childBranch = objectForCommonPattern;
                                }
                                else if (thisNode.parentBranch.anotherBranch == thisNode)    //thisNode is anotherBranch of a parent Node
                                {
                                    thisNode.parentBranch.anotherBranch = objectForCommonPattern;
                                }
                                else
                                {
                                    throw new ApplicationException(message: "Unpredictable behaviour: logical error_3.");
                                }

                                objectForCommonPattern.parentBranch = thisNode.parentBranch;
                                objectForCommonPattern.anotherBranch = thisNode.anotherBranch;
                                objectForCommonPattern.childBranch = thisNode;
                                thisNode.parentBranch = objectForCommonPattern;
                                thisNode.anotherBranch = objectForNewPattern;
                                objectForNewPattern.parentBranch = thisNode;

                                if (objectForCommonPattern.anotherBranch != null)
                                {
                                    objectForCommonPattern.anotherBranch.parentBranch = objectForCommonPattern;
                                }
                                return CompletionStatus.NEW_ADDED;
                            }
                        }
                    }
                }
                else
                {
                    if (thisNode.anotherBranch != null) thisNode = thisNode.anotherBranch;
                    else break;
                }
            }
            if (thisNode.anotherBranch == null)
            {
                thisNode.anotherBranch = new Node();
                patternToAdd = patternToAdd.Substring(currentPattern.Length,
                                                      patternToAdd.Length - currentPattern.Length);
                thisNode.anotherBranch.SetValue(patternToAdd);
                thisNode.anotherBranch.SetParentNode(thisNode);
                return CompletionStatus.NEW_ADDED;
            }
            else return CompletionStatus.NULL;
        }

    }
}
