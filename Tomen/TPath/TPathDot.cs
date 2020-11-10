namespace Tomen {
	internal class TPathDot : TPath {
		private TPath result;
		private TPath pathExpression;

		public TPathDot(TPath result, TPath pathExpression) {
			this.result = result;
			this.pathExpression = pathExpression;
		}

		public TomlValue Eval(TomlValue table) {
			return pathExpression.Eval(result.Eval(table));
		}
	}
}