using System;
using System.Linq;
using System.Collections.Generic;


namespace PrefixTrieLibrary
{
    public class Node
    {   
        Node anotherBranch;
        Node childBranch;
        Node parentBranch;
        string value;
        List<int> wordsInNodeIndexed; //It is needed to make a tree look
                                           //more "simple"; example: if u add words
                                           //ad, advert, advertisment, then
                                           //a corresponding Node will look like ad*vert*isement*  
        public enum CompletionStatus
        {
            NEW_ADDED = 1,
            ALREADY_EXISTS = 2,
            NULL = 0
        }
        public Node()
        {
            anotherBranch = childBranch = parentBranch = null;
            value = "";
            wordsInNodeIndexed = new List<int>();
        }
        public void SetValue(string valueContainer)
        {
            this.value = valueContainer;
        }

        public string GetValue()
        {
            return this.value;
        }

        public void SetParentNode(Node parentNodeContainer)
        {
            this.parentBranch = parentNodeContainer;
        }

        public Node GetParentNode()
        {
            return this.parentBranch;
        }
        public bool Contains(string patternToCheck, string followingLevelPattern = "")
        {
            Node thisNode = this;
            if(this.value == "")
            {
                if(this.childBranch == null)
                {
                    return false;
                }
                thisNode = this.childBranch;
            }

            string currentPattern = followingLevelPattern;
            while(thisNode != null)
            {
                if(patternToCheck.StartsWith(currentPattern + thisNode.value[0])) //if matches a single char ->
                {    
                    currentPattern += thisNode.value;
                    if(currentPattern.StartsWith(patternToCheck))
                    {
                        int checkIndex = currentPattern.Length - thisNode.value.Length;
                        foreach(int word in thisNode.wordsInNodeIndexed)
                        {
                            if( (checkIndex + word) == patternToCheck.Length) return true;
                        }
                        if(currentPattern == patternToCheck && thisNode.childBranch == null)
                        {
                            return true;
                        }
                    }
                    else if(patternToCheck.StartsWith(currentPattern))
                    {
                        Node iteratingNode = thisNode.childBranch;
                        if(iteratingNode == null)
                        {
                            return false;
                        }
                        
                        do
                        {
                            if(patternToCheck.StartsWith(currentPattern + iteratingNode.value[0]))
                            {       
                                return iteratingNode.Contains(patternToCheck, currentPattern);
                            }
                            iteratingNode = (iteratingNode.anotherBranch == null)?
                                                iteratingNode:iteratingNode.anotherBranch;
                        }
                        while(iteratingNode.anotherBranch != null); //While there exists another child

                        if(iteratingNode.anotherBranch == null)
                        {
                            return false;
                        }   
                    }
                }
                else
                {
                    if(thisNode.anotherBranch != null) thisNode = thisNode.anotherBranch;
                    else break;
                }
            }
            return false;
        }
        public CompletionStatus AddNode(string patternToAdd, string followingLevelPattern = "") //brainfuck
        {   
            Node thisNode = this; //It is needed to operate within this
                                  //method in a more sophisticated way.
            if(this.value == "")
            {
                if(this.childBranch == null)
                {
                    this.childBranch = new Node();
                    this.childBranch.SetValue(patternToAdd);
                    this.childBranch.SetParentNode(this);
                    return CompletionStatus.NEW_ADDED;
                }
                thisNode = this.childBranch;
            }

            string currentPattern = followingLevelPattern;
            while(thisNode != null)
            {
                if(patternToAdd.StartsWith(currentPattern + thisNode.value[0])) //if matches a single char ->
                {                                                               //its place is in this branch
                    currentPattern += thisNode.value;
/*Section ok*/      if(currentPattern.StartsWith(patternToAdd)) //Checking if pattern is itself
                    {                                           //a prefix match for current node.
                        //evaluate position of a word to add it into a list of existing words
                        int rest = thisNode.value.Length - (currentPattern.Length - patternToAdd.Length);
                        if(thisNode.childBranch == null && rest == thisNode.value.Length)
                        {
                            return CompletionStatus.ALREADY_EXISTS;
                        }
                        else if(!thisNode.wordsInNodeIndexed.Contains(rest))
                        {
                            thisNode.wordsInNodeIndexed.Add(rest);
                            return CompletionStatus.NEW_ADDED;
                        }
                        return CompletionStatus.ALREADY_EXISTS;
                    }
                    else if(patternToAdd.StartsWith(currentPattern)) /*exactly starts with pattern*/               
                    {                                                //example: ADVERTisement:ADVERT
                        Node iteratingNode = thisNode.childBranch; //needed as an iterator
                        if(iteratingNode == null)
                        {
                            //No children were added yet; 
                            //adding pattern piece to thisNode.
                            //Hope this works...
                            patternToAdd = patternToAdd.Substring( currentPattern.Length,
                                                                   patternToAdd.Length-currentPattern.Length);
                            thisNode.wordsInNodeIndexed.Add(thisNode.value.Length);
                            thisNode.SetValue(thisNode.value + patternToAdd);
                            return CompletionStatus.NEW_ADDED;
                        }
                        
                        do //If there is any number of children, except zero - we seek among
                           //each for a first char match -> then calling method recursively.
                        {   
                            if(patternToAdd.StartsWith(currentPattern + iteratingNode.value[0]))
                            {   //There exists a child with a match,
                                //now must be a descent down into a child branch, exact beginning
                                return iteratingNode.AddNode(patternToAdd, currentPattern);
                                 //other children are not needed to be looked through
                            }
                            iteratingNode = (iteratingNode.anotherBranch == null)?
                                             iteratingNode:iteratingNode.anotherBranch;
                        }
                        while(iteratingNode.anotherBranch != null); //While there exists another child

                        if(iteratingNode.anotherBranch == null)
                        {   //If yet no branch match found
                            //we delete first currentPattern.Length characters from patternToAdd.
                            //Example: currentPattern = "Good"
                            //         patternToAdd =   "Good morning" => patternToAdd = " morning"

                            patternToAdd = patternToAdd.Substring( currentPattern.Length,
                                                                   patternToAdd.Length-currentPattern.Length);
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
                        
                        for(string str = normalPrefixPattern; str.Length > 1; str = str.Remove(str.Length-1))
                        {
                            if(currentPattern.StartsWith(str))
                            {
                                //str = pattern that is match for currentPattern
                                //mismatchedRest = rest that is gonna be added
                                string mismatchedRest = patternToAdd.Substring(str.Length, patternToAdd.Length-str.Length);
                                
                                Node objectForNewPattern = new Node();
                                objectForNewPattern.SetValue(mismatchedRest);                           //First setNode
                                
                                Node objectForCommonPattern = new Node();
                                int loopCount = normalPrefixPattern.Length - str.Length;
                                
                                int commonPatternLength = normalPrefixPattern.Length - loopCount;
                                if(commonPatternLength < 0 || !(commonPatternLength < thisNode.value.Length))
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
                                                                           thisNode.value.Length-commonPatternLength);
                                thisNode.SetValue(leftover);                                            //Third setNode

                                if(thisNode.wordsInNodeIndexed.Any()) //If there is any list member -> reevaluate it's position
                                {
                                    objectForCommonPattern.wordsInNodeIndexed = thisNode.wordsInNodeIndexed
                                                                                .Where(word => (word <= commonPatternLength)).ToList();

                                    thisNode.wordsInNodeIndexed.RemoveAll(word => (word <= commonPatternLength));
                                    thisNode.wordsInNodeIndexed = thisNode.wordsInNodeIndexed
                                                                  .Select(word => {word -= commonPatternLength; return word;}).ToList();
                                }
                                
                                if(thisNode.parentBranch.childBranch == thisNode) //thisNode is childBranch of a parentNode
                                {
                                    thisNode.parentBranch.childBranch = objectForCommonPattern;      
                                }
                                else if(thisNode.parentBranch.anotherBranch == thisNode)    //thisNode is anotherBranch of a parent Node
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
                                thisNode.parentBranch = objectForCommonPattern; //I indicate this line as being not a needed one for every case, however
                                thisNode.anotherBranch = objectForNewPattern;
                                objectForNewPattern.parentBranch = thisNode;
                                
                                if(objectForCommonPattern.anotherBranch != null)
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
                    if(thisNode.anotherBranch != null) thisNode = thisNode.anotherBranch;
                    else break;
                }
            }
            if(thisNode.anotherBranch == null)
            {
                thisNode.anotherBranch = new Node();
                patternToAdd = patternToAdd.Substring( currentPattern.Length,
                                                       patternToAdd.Length-currentPattern.Length); //right
                thisNode.anotherBranch.SetValue(patternToAdd);
                thisNode.anotherBranch.SetParentNode(thisNode);
                return CompletionStatus.NEW_ADDED;
            }
            else return CompletionStatus.NULL;
        }
    }
}
