﻿/* Copyright 2010 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.Bson.IO {
    public class BsonJsonReaderBookmark : BsonReaderBookmark {
        #region private fields
        private BsonJsonReaderContext context;
        private JsonToken currentToken;
        private BsonValue currentValue;
        private JsonToken pushedToken;
        private int position;
        #endregion

        #region constructors
        internal BsonJsonReaderBookmark(
            BsonReadState state,
            BsonType currentBsonType,
            string currentName,
            BsonJsonReaderContext context,
            JsonToken currentToken,
            BsonValue currentValue,
            JsonToken pushedToken,
            int position
        )
            : base(state, currentBsonType, currentName) {
            this.context = context.Clone();
            this.currentToken = currentToken;
            this.currentValue = currentValue;
            this.pushedToken = pushedToken;
            this.position = position;
        }
        #endregion

        #region internal properties
        internal BsonJsonReaderContext Context {
            get { return context; }
        }

        public JsonToken CurrentToken {
            get { return currentToken; }
        }

        public BsonValue CurrentValue {
            get { return currentValue; }
        }

        public int Position {
            get { return position; }
        }

        public JsonToken PushedToken {
            get { return pushedToken; }
        }
        #endregion
    }
}
