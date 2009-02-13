using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tiestvilee.hisp.parser
{
    public class WhitespaceToBrackets
    {
        public string convert(string s)
        {
            StringBuilder result = new StringBuilder(s.Length*2);
            State state = State.LookingForLeftBracket;
            int indent = 0;
            int depth = 0;
            int currentIndent = 0;
            foreach(char c in s.Trim())
            {
                switch (state)
                {
                    case State.LookingForLeftBracket:
                        if (c != '<')
                        {
                            result.Append('<');
                        }
                        depth += 1;
                        state = State.InText;
                        break;
                    case State.InText:

                        depth = UpdateDepthBasedOnBrackets(c, result, depth);

                        if(c == '\r' || c == '\n')
                        {
                            state = State.InIndent;
                            currentIndent = 0;
                        }

                        break;

                    case State.InIndent:
                        if (c == '\r' || c == '\n')
                        {
                            currentIndent = 0;
                        } else if (c == ' ')
                        {
                            currentIndent += 1;
                        } else
                        {
                            state = State.InText;
                            depth = UpdateBracketsBasedOnIndent(result, depth, currentIndent, indent);
                            indent = currentIndent;
                            depth = UpdateDepthBasedOnBrackets(c, result, depth);

                        }
                        break;
                }
                result.Append(c);
            }

            for(int i=depth; i>0; i--)
            {
                result.Append('>');
            }
            return result.ToString();
        }

        private int UpdateBracketsBasedOnIndent(StringBuilder result, int depth, int currentIndent, int indent)
        {
            if(currentIndent < indent)
            {
                for(;indent > currentIndent; indent -= 4)
                {
                    result.Append('>');
                    depth -= 1;
                    //
                }
                result.Append("><");
                depth += 1;
            }
            else if (currentIndent > indent)
            {
                for (;indent < currentIndent; indent += 4)
                {
                    result.Append('<');
                    depth += 1;
                }
            }
            else
            {
                result.Append("><");
            }
            return depth;
        }

        private int UpdateDepthBasedOnBrackets(char c, StringBuilder result, int depth)
        {
            if (c == '>')
            {
                depth -= 1;
            } else if (c == '<')
            {
                depth += 1;
            }
            return depth;
        }

        private enum State
        {
            LookingForLeftBracket,
            InText,
            InIndent
        }
    }
}
