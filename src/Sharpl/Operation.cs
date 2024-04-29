using System.Diagnostics.Contracts;

namespace Sharpl {
    public readonly struct Operation {
        public enum T {
            Push,
            Stop
        };

        public readonly dynamic Data;
        public T Type { get; }

        public Operation(T type, dynamic data) {
            this.Type = type;
            this.Data = data;
        }
    }
}