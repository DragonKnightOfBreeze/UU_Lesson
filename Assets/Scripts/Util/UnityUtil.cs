/*******
 * ［前置］
 * 项目：UU课堂
 * 作者：微风的龙骑士 风游迩
 * 
 * ［标题］
 * 
 * 
 * ［功能］
 * 
 * 
 * ［思路］
 * 
 * 
 * ［用法］
 * 
 */
namespace Util {
	/// <summary></summary>
	public static class Unityutil {
		public static int ToInt(this string str) {
			var temp = 0;
			int.TryParse(str, out temp);
			return temp;
		}
	}
}