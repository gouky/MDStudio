using System;
using System.Diagnostics;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor
{
    /// <summary>
    /// Helper functions for manipulating text.
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// Types of characters
        /// </summary>
        private enum CharacterType
        {
            /// <summary>
            /// A letter, digit or underscore.
            /// </summary>
            LetterDigitOrUnderscore,
            /// <summary>
            /// A whitespace character.
            /// </summary>
            WhiteSpace,
            /// <summary>
            /// Any other character (no letter, digit, or whitespace).
            /// </summary>
            Other
        }


        /// <summary>
        /// Determines whether a character is a letter, a digit, or an underscore.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="c"/> is a letter, a digit, or an underscore; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsLetterDigitOrUnderscore(char c)
        {
            return Char.IsLetterOrDigit(c) || (c == '_');
        }


        /// <summary>
        /// Gets the type of the character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The character type.</returns>
        private static CharacterType GetCharacterType(char c)
        {
            if (IsLetterDigitOrUnderscore(c))
                return CharacterType.LetterDigitOrUnderscore;
            else if (Char.IsWhiteSpace(c))
                return CharacterType.WhiteSpace;
            else
                return CharacterType.Other;
        }


        /// <summary>
        /// Gets the line of the document as <see cref="string"/>.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>The line as <see cref="string"/>.</returns>
        public static string GetLineAsString(IDocument document, int lineNumber)
        {
            LineSegment line = document.GetLineSegment(lineNumber);
            return document.GetText(line.Offset, line.Length);
        }


        /// <summary>
        /// Determines whether a line of a document is empty (no characters or whitespaces).
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>
        /// <see langword="true"/> if line is empty of filled with whitespaces; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsEmptyLine(IDocument document, int lineNumber)
        {
            return IsEmptyLine(document, document.GetLineSegment(lineNumber));
        }


        /// <summary>
        /// Determines whether a line of a document is empty (no characters or whitespaces).
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="line">The line.</param>
        /// <returns>
        /// 	<see langword="true"/> if line is empty of filled with whitespaces; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsEmptyLine(IDocument document, LineSegment line)
        {
            int lineOffset = line.Offset;
            int startOffset = lineOffset;
            int endOffset = lineOffset + line.Length;
            for (int i = startOffset; i < endOffset; ++i)
            {
                char ch = document.GetCharAt(i);
                if (!Char.IsWhiteSpace(ch))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Gets the offset of the first non-whitespace character.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset where to start the search.</param>
        /// <returns>
        /// The offset of the first non-whitespace at or after <paramref name="offset"/>.
        /// <see cref="IDocument.TextLength"/> is returned if no non-whitespace is found.
        /// </returns>
        public static int FindFirstNonWhitespace(IDocument document, int offset)
        {
            while (offset < document.TextLength && Char.IsWhiteSpace(document.GetCharAt(offset)))
                ++offset;

            return offset;
        }


        /// <summary>
        /// Finds the offset of the opening bracket in the block defined by offset skipping
        /// brackets, strings and comments.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset of an position in the block (before the closing bracket).</param>
        /// <param name="openBracket">The character for the opening bracket.</param>
        /// <param name="closingBracket">The character for the closing bracket.</param>
        /// <returns>
        /// Returns the offset of the opening bracket or -1 if no matching bracket was found.
        /// </returns>
        public static int FindOpeningBracket(IDocument document, int offset, char openBracket, char closingBracket)
        {
            return document.FormattingStrategy.SearchBracketBackward(document, offset, openBracket, closingBracket);
        }


        /// <summary>
        /// Finds the offset of the closing bracket in the block defined by offset skipping
        /// brackets, strings and comments.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset of an position in the block (after the opening bracket).</param>
        /// <param name="openBracket">The character for the opening bracket.</param>
        /// <param name="closingBracket">The character for the closing bracket.</param>
        /// <returns>
        /// Returns the offset of the closing bracket or -1 if no matching bracket was found.
        /// </returns>
        public static int FindClosingBracket(IDocument document, int offset, char openBracket, char closingBracket)
        {
            return document.FormattingStrategy.SearchBracketForward(document, offset, openBracket, closingBracket);
        }


        /// <summary>
        /// Gets the identifier at the given offset in the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset of part of the identifier.</param>
        /// <returns>The identifier at <paramref name="offset"/>.</returns>
        /// <remarks>
        /// An identifier is a single word consisting of letters, digits, or underscores.
        /// An identifier must start with a letter or underscore.
        /// </remarks>
        public static string GetIdentifierAt(IDocument document, int offset)
        {
            if (offset < 0 || offset >= document.TextLength || !IsPartOfIdentifier(document.GetCharAt(offset)))
                return String.Empty;

            int startOffset = FindStartOfIdentifier(document, offset);
            if (startOffset == -1)
                return String.Empty;

            int endOffset = FindEndOfIdentifier(document, offset);

            Debug.Assert(endOffset != -1);
            Debug.Assert(endOffset >= startOffset);

            return document.GetText(startOffset, endOffset - startOffset + 1);
        }


        private static bool IsPartOfIdentifier(char c)
        {
            return IsLetterDigitOrUnderscore(c);
        }


        /// <summary>
        /// Finds the start of the identifier at the given offset.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The offset of the first character of the identifier; or -1 if there is no
        /// identifier at the specified offset.
        /// </returns>
        /// <remarks>
        /// <para>
        /// An identifier is a single word consisting of letters, digits, or underscores.
        /// An identifier must start with a letter or underscore.
        /// </para>
        /// </remarks>
        public static int FindStartOfIdentifier(IDocument document, int offset)
        {
            if (offset < 0 || document.TextLength <= offset)
                return -1;

            if (!IsPartOfIdentifier(document.GetCharAt(offset)))
            {
                // Character at offset is does not belong to an identifier.
                return -1;
            }

            // Search backwards
            LineSegment line = document.GetLineSegmentForOffset(offset);
            int lineOffset = line.Offset;
            while (offset > lineOffset && IsPartOfIdentifier(document.GetCharAt(offset - 1)))
                --offset;

            // Check if first character is the start of an identifier.
            // (We need to make sure that it is not a number.)
            char startCharacter = document.GetCharAt(offset);
            if (Char.IsLetter(startCharacter) || startCharacter == '_')
                return offset;
            else
                return -1;
        }


        /// <summary>
        /// Finds the end of the identifier at the given offset.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The offset of the last character of the identifier; or -1 if there is no
        /// identifier at the specified offset.
        /// </returns>
        /// <remarks>
        /// <para>
        /// An identifier is a single word consisting of letters, digits, or underscores.
        /// An identifier must start with a letter or underscore.
        /// </para>
        /// <para>
        /// <strong>Important: </strong>This method does not guarantee that the word
        /// at <paramref name="offset"/> is an identifier - it could also be a number instead of
        /// an identifier. To make sure that the current word is really an identifier, you should 
        /// search for the start of the identifier and check whether it starts with a letter or 
        /// underscore.
        /// </para>
        /// </remarks>
        public static int FindEndOfIdentifier(IDocument document, int offset)
        {
            if (!IsPartOfIdentifier(document.GetCharAt(offset)))
            {
                // Character at offset is does not belong to an identifier.
                return -1;
            }

            // Search forward
            LineSegment line = document.GetLineSegmentForOffset(offset);
            int lineEnd = line.Offset + line.Length;
            while (offset + 1 < lineEnd && IsPartOfIdentifier(document.GetCharAt(offset + 1)))
                ++offset;

            return offset;
        }


        /// <summary>
        /// Finds the start offset the word at or before the specified offset.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The start of the word before <paramref name="offset"/>.</returns>
        public static int FindPrevWordStart(IDocument document, int offset)
        {
            // go back to the start of the word we are on
            // if we are already at the start of a word or if we are in whitespace, then go back
            // to the start of the previous word

            if (offset > 0)
            {
                LineSegment line = document.GetLineSegmentForOffset(offset);
                CharacterType charType = GetCharacterType(document.GetCharAt(offset - 1));
                int lineOffset = line.Offset;
                while (offset > lineOffset && GetCharacterType(document.GetCharAt(offset - 1)) == charType)
                    --offset;

                // if we were in whitespace, and now we're at the end of a word or operator, go back to the beginning of it
                if (charType == CharacterType.WhiteSpace && offset > lineOffset)
                {
                    charType = GetCharacterType(document.GetCharAt(offset - 1));
                    while (offset > lineOffset && GetCharacterType(document.GetCharAt(offset - 1)) == charType)
                        --offset;
                }
            }

            return offset;
        }


        /// <summary>
        /// Finds the offset where the next word starts.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The start of the next word after <paramref name="offset"/>.</returns>
        public static int FindNextWordStart(IDocument document, int offset)
        {
            // go forward to the start of the next word
            // if the cursor is at the start or in the middle of a word we move to the end of the word
            // and then past any whitespace that follows it
            // if the cursor is at the start or in the middle of some whitespace we move to the start of the
            // next word

            LineSegment line = document.GetLineSegmentForOffset(offset);
            int endPos = line.Offset + line.Length;
            // lets go to the end of the word, whitespace or operator
            CharacterType t = GetCharacterType(document.GetCharAt(offset));
            while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == t)
                ++offset;

            // now we're at the end of the word, lets find the start of the next one by skipping whitespace
            while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == CharacterType.WhiteSpace)
                ++offset;

            return offset;
        }

        /// <summary>
        /// Finds the offset where the string starts.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The start of the string.</returns>
        public static int FindStringStart(IDocument document, int offset)
        {
            // go forward to the start of the string
            // if the cursor is at the end or in the middle of a string we move to the start of the string

            if (offset > 0)
            {
                LineSegment line = document.GetLineSegmentForOffset(offset);
                int lineOffset = line.Offset;

                // We are still in string, so we decrement offset.
                while (offset > lineOffset && document.GetCharAt(offset - 1) != '"')
                    --offset;

                // If we didn't find start of the string we make offset -1.
                if (offset <= lineOffset)
                    offset = -1;
            }
            else
                offset = -1;

            return offset;
        }

        /// <summary>
        /// Finds the offset where the string ends.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The end of the string.</returns>
        public static int FindStringEnd(IDocument document, int offset)
        {
            // go forward to the end of the string
            // if the cursor is at the start or in the middle of a string we move to the end of the string

            LineSegment line = document.GetLineSegmentForOffset(offset);
            int endPos = line.Offset + line.Length;

            // We are still in string, so we increment offset.
            while (offset < endPos && document.GetCharAt(offset) != '"')
                ++offset;

            // If we didn't find end of the string we make offset -1.
            if (offset >= endPos)
                offset = -1;

            return offset;
        }


        /// <summary>
        /// Gets the word before caret.
        /// </summary>
        /// <returns>The word.</returns>
        /// <remarks>
        /// The 
        /// </remarks>
        public static string GetWordBeforeCaret(TextArea textArea)
        {
            int start = FindPrevWordStart(textArea.Document, textArea.Caret.Offset);
            return textArea.Document.GetText(start, textArea.Caret.Offset - start);
        }


        /// <summary>
        /// Deletes the word before caret.
        /// </summary>
        /// <returns>The new offset of the caret.</returns>
        public static int DeleteWordBeforeCaret(TextArea textArea)
        {
            int start = FindPrevWordStart(textArea.Document, textArea.Caret.Offset);
            textArea.Document.Remove(start, textArea.Caret.Offset - start);
            return start;
        }


        /// <summary>
        /// Gets the expression before a given offset.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="initialOffset">The initial offset.</param>
        /// <returns>The expression.</returns>
        /// <remarks>
        /// This method returns the expression before a specified offset.
        /// That method is used in code completion to determine the expression before
        /// the caret. The expression can be passed to a parser to resolve the type
        /// or similar.
        /// </remarks>
        public static string GetExpressionBeforeOffset(IDocument document, int initialOffset)
        {
            int offset = initialOffset;
            while (offset - 1 > 0)
            {
                switch (document.GetCharAt(offset - 1))
                {
                    case '\n':
                    case '\r':
                    case '}':
                        goto done;
                    //						offset = FindOpeningBracket(document, offset - 2, '{','}');
                    //						break;
                    case ']':
                        offset = FindOpeningBracket(document, offset - 2, '[', ']');
                        break;
                    case ')':
                        offset = FindOpeningBracket(document, offset - 2, '(', ')');
                        break;
                    case '.':
                        --offset;
                        break;
                    case '"':
                        if (offset < initialOffset - 1)
                        {
                            return null;
                        }
                        return "\"\"";
                    case '\'':
                        if (offset < initialOffset - 1)
                        {
                            return null;
                        }
                        return "'a'";
                    case '>':
                        if (document.GetCharAt(offset - 2) == '-')
                        {
                            offset -= 2;
                            break;
                        }
                        goto done;
                    default:
                        if (Char.IsWhiteSpace(document.GetCharAt(offset - 1)))
                        {
                            --offset;
                            break;
                        }
                        int start = offset - 1;
                        if (!IsLetterDigitOrUnderscore(document.GetCharAt(start)))
                        {
                            goto done;
                        }

                        while (start > 0 && IsLetterDigitOrUnderscore(document.GetCharAt(start - 1)))
                        {
                            --start;
                        }
                        string word = document.GetText(start, offset - start).Trim();
                        switch (word)
                        {
                            case "ref":
                            case "out":
                            case "in":
                            case "return":
                            case "throw":
                            case "case":
                                goto done;
                        }

                        if (word.Length > 0 && !IsLetterDigitOrUnderscore(word[0]))
                        {
                            goto done;
                        }
                        offset = start;
                        break;
                }
            }
        done:
            // simple exit fails when : is inside comment line or any other character
            // we have to check if we got several ids in resulting line, which usually happens when
            // id. is typed on next line after comment one
            // Would be better if lexer would parse properly such expressions. However this will cause
            // modifications in this area too - to get full comment line and remove it afterwards
            if (offset < 0)
                return string.Empty;

            string resText = document.GetText(offset, initialOffset - offset).Trim();
            int pos = resText.LastIndexOf('\n');
            if (pos >= 0)
            {
                offset += pos + 1;
                // whitespaces and tabs, which might be inside, will be skipped by trim below
            }
            string expression = document.GetText(offset, initialOffset - offset).Trim();
            return expression;
        }


        /// <summary>
        /// Checks whether a region (offset + length) matches a given word.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="segment">The segment of the document to compare against <paramref name="word"/>.</param>
        /// <param name="word">The word.</param>
        /// <returns><see langword="true"/> if region matches word.</returns>
        /// <remarks>
        /// The comparison is case-sensitive.
        /// </remarks>
        public static bool CompareSegment(IDocument document, ISegment segment, string word)
        {
            return CompareSegment(document, segment.Offset, segment.Length, word);
        }


        /// <summary>
        /// Checks whether a region (offset + length) matches a given word.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="segment">The segment of the document to compare against <paramref name="word"/>.</param>
        /// <param name="word">The word.</param>
        /// <param name="caseSensitive">If set to <see langword="true"/> the comparison is case-sensitive.</param>
        /// <returns><see langword="true"/> if region matches word.</returns>
        public static bool CompareSegment(IDocument document, ISegment segment, string word, bool caseSensitive)
        {
            return CompareSegment(document, segment.Offset, segment.Length, word, caseSensitive);
        }


        /// <summary>
        /// Checks whether a region (offset + length) matches a given word.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="word">The word.</param>
        /// <returns><see langword="true"/> if region matches word.</returns>
        /// <remarks>
        /// The comparison is case-sensitive.
        /// </remarks>
        public static bool CompareSegment(IDocument document, int offset, int length, string word)
        {
            if (length != word.Length || document.TextLength < offset + length)
                return false;

            for (int i = 0; i < length; ++i)
                if (document.GetCharAt(offset + i) != word[i])
                    return false;

            return true;
        }


        /// <summary>
        /// Checks whether a region (offset + length) matches a given word.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="word">The word.</param>
        /// <param name="caseSensitive">If set to <see langword="true"/> the comparison is case-sensitive.</param>
        /// <returns><see langword="true"/> if region matches word.</returns>
        public static bool CompareSegment(IDocument document, int offset, int length, string word, bool caseSensitive)
        {
            if (caseSensitive)
                return CompareSegment(document, offset, length, word);

            if (length != word.Length || document.TextLength < offset + length)
                return false;

            for (int i = 0; i < length; ++i)
                if (Char.ToUpper(document.GetCharAt(offset + i)) != Char.ToUpper(word[i]))
                    return false;

            return true;
        }


        /// <summary>
        /// Converts leading whitespaces to tabs.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="tabIndent">The indentation size.</param>
        /// <returns>The converted line.</returns>
        /// <remarks>
        /// This function takes a string and converts the whitespace in front of
        /// it to tabs. If the length of the whitespace at the start of the string
        /// was not a whole number of tabs then there will still be some spaces just
        /// before the text starts.
        /// the output string will be of the form:
        /// <list type="number">
        /// <item><description>zero or more tabs</description></item>
        /// <item><description>zero or more spaces (less than tabIndent)</description></item>
        /// <item><description>the rest of the line</description></item>
        /// </list>
        /// </remarks>
        public static string LeadingWhitespaceToTabs(string line, int tabIndent)
        {
            StringBuilder sb = new StringBuilder(line.Length);
            int consecutiveSpaces = 0;
            int i;
            for (i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                {
                    consecutiveSpaces++;
                    if (consecutiveSpaces == tabIndent)
                    {
                        sb.Append('\t');
                        consecutiveSpaces = 0;
                    }
                }
                else if (line[i] == '\t')
                {
                    sb.Append('\t');
                    // if we had say 3 spaces then a tab and tabIndent was 4 then
                    // we would want to simply replace all of that with 1 tab
                    consecutiveSpaces = 0;
                }
                else
                {
                    break;
                }
            }

            if (i < line.Length)
                sb.Append(line.Substring(i - consecutiveSpaces));

            return sb.ToString();
        }
    }
}
