using System;
using System.Collections.Generic;
using System.Text;

namespace LibNbt.Queries
{
    public class TagQuery
    {
        public string Query { get; protected set; }
        protected List<TagQueryToken> Tokens { get; set; }
        protected int CurrentTokenIndex { get; set; }

        public TagQuery() : this("") { }
        public TagQuery(string query)
        {
            Tokens = new List<TagQueryToken>();

            if (!string.IsNullOrEmpty(query))
            {
                SetQuery(query);
            }
        }

        public void SetQuery(string query)
        {
            Query = query;

            if (string.IsNullOrEmpty(Query)) { throw new ArgumentException("You must provide a query.", "query"); }
            if (!Query.StartsWith("/")) { throw new ArgumentException("The query must begin with a \"/\".", "query"); }

            Tokens.Clear();
            CurrentTokenIndex = -1;

            var escapingChar = false;
            TagQueryToken token = null;
            var sbToken = new StringBuilder();
            for (var idx = 0; idx < query.Length; idx++)
            {
                if (escapingChar)
                {
                    sbToken.Append(query[idx]);
                    escapingChar = false;
                    continue;
                }

                if (query[idx] == '/')
                {
                    if (token != null)
                    {
                        token.Name = sbToken.ToString();
                        Tokens.Add(token);
                    }

                    token = new TagQueryToken {Query = this};
                    sbToken.Length = 0;
                    continue;
                }

                if (query[idx] == '\\')
                {
                    escapingChar = true;
                    continue;
                }

                sbToken.Append(query[idx]);
            }
            if (token != null)
            {
                token.Name = sbToken.ToString();
                Tokens.Add(token);
            }
        }

        /// <summary>
        /// The total number of tokens in the query.
        /// </summary>
        /// <returns>The number of tokens</returns>
        public int Count() { return Tokens.Count; }
        /// <summary>
        /// The number of tokens left in the query after the current one.
        /// </summary>
        /// <returns>The number of tokens</returns>
        public int TokensLeft() { return Count() - (CurrentTokenIndex + 1); }
        public void MoveFirst()
        {
            CurrentTokenIndex = -1;
        }
        public TagQueryToken Previous()
        {
            if (CurrentTokenIndex >= 0)
            {
                return Tokens[--CurrentTokenIndex];
            }
            return null;
        }
        public TagQueryToken Next()
        {
            if (CurrentTokenIndex + 1 < Count())
            {
                return Tokens[++CurrentTokenIndex];
            }
            return null;
        }
        public TagQueryToken Peek()
        {
            if (CurrentTokenIndex + 1 < Count())
            {
                return Tokens[CurrentTokenIndex + 1];
            }
            return null;
        }
    }
}
