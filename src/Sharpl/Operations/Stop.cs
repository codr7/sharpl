namespace Sharpl.Operations {
    public readonly record struct Stop() {
        public static Operation instance = new Operation(Operation.T.Stop, new Stop());

        public static Operation Make() {
            return instance;
        }
    }
}