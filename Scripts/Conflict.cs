using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hibzz.Merge
{
    public class Conflict
    {
        public string ObjectId;
        public string VariableName;
        public string Current;
        public string Remote;

		public override string ToString()
		{
            return $"{VariableName}[{ObjectId}] - Current: {Current} - Remote: {Remote}";
		}
	}
}
