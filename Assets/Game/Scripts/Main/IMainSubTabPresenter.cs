using Cysharp.Threading.Tasks;

namespace Main
{
	public record MainSubTabReturnType
	{
		public record Switch(MainState State) : MainSubTabReturnType;
		public record Close() : MainSubTabReturnType;
	}

	public record MainSubTabReturn(MainSubTabReturnType Type);
}