//核心层，对话数据管理器（类）
//作用：根据“对话数据格式”定义，输入段落编号，输出给定的对话内容（对话双方，人名，内容）
//单输入，单输出，这样耦合性比较低

using System.Collections.Generic;

namespace Kernel {
	/// <summary>对话双方的枚举</summary>
	public enum DialogSide {
		None,
		HeroSide, //英雄
		NPCSide   //NPC
	}

	//为什么这里加public会出现参数可访问性错误？？？

	internal class DialogDataMgr {
		private static DialogDataMgr _Instance;                     //本类的实例
		private static List<DialogDataFormat> _AllDialogDataArray;  //所有的对话数据集合
		private static List<DialogDataFormat> _CurDialogCacheArray; //当前对话缓存的集合
		private static int _IntIndexByDialogSection;                //对话序号（某个段落）

		//原对话“段落编号”(Add Function:后来增加的内容)
		private static int _OriginalDialogSectionNum = 1;


		/// <summary>构造方法</summary>
		private DialogDataMgr() {
			//实例化字段信息
			_AllDialogDataArray = new List<DialogDataFormat>();
			_CurDialogCacheArray = new List<DialogDataFormat>();
			_IntIndexByDialogSection = 0;
		}

		/// <summary>得到类的实例</summary>
		/// <returns></returns>
		public static DialogDataMgr GetInstance() {
			if(_Instance == null)
				_Instance = new DialogDataMgr();
			return _Instance;
		}


		/// <summary>公共方法：加载外部数据集合 通过这个方法来控制要加载的XML文件 （使用布尔值来测试方法有没有成功）</summary>
		/// <param name="dialogDataArray">外部数据集合</param>
		/// <returns>true: 表明数据加载成功；	</returns>
		public bool LoadAllDialogData(List<DialogDataFormat> dialogDataArray) {
			//参数检查
			if(dialogDataArray == null || dialogDataArray.Count == 0)
				return false;
			///加载数据（列表存在，且长度为0）
			if(_AllDialogDataArray != null && _AllDialogDataArray.Count == 0) {
				for(var i = 0; i < dialogDataArray.Count; i++)
					_AllDialogDataArray.Add(dialogDataArray[i]);
				return true;
			}
			return false;
		}


		/// <summary>得到下一条对话记录</summary>
		/// <param name="diaSectionNum">输入：段落编号</param>
		/// <param name="diaSide">输出：对话方</param>
		/// <param name="diaPerson">输出：对话人名</param>
		/// <param name="diaContent">输出：对话内容</param>
		/// <returns>true: 输出了合法的对话数据</returns>
		public bool GetNextDialogInfoRecoder(int diaSectionNum, out DialogSide diaSide, out string diaPerson, out string diaContent) {
			//显示默认内容
			diaSide = DialogSide.None;
			diaPerson = "[GetNextDialogInfoRecoder()/diaPerson]";
			diaContent = "[GetNextDialogInfoRecoder()/diaContent]";

			//输出参数检查
			if(diaSectionNum < 0)
				return false;
			//段落编号增大后，需要保留上一个“对话段落编号”，以方便后续逻辑处理
			//###改进### 只要是不相等。这样可以乱序访问

			if(diaSectionNum != _OriginalDialogSectionNum) {
				//重置“内部序号”（后续应该会执行一个自加操作）
				_IntIndexByDialogSection = 0;
				//清空“对话缓存”（已经是下一个了）
				_CurDialogCacheArray.Clear();
				//把当前的段落编号记录下来（不一定非要是连续的段落编号）
				_OriginalDialogSectionNum = diaSectionNum;
			}

			//如果当前缓存不为空
			if(_CurDialogCacheArray != null && _CurDialogCacheArray.Count > 0) {
				if(_IntIndexByDialogSection < _CurDialogCacheArray.Count)
					++_IntIndexByDialogSection; //自增索引
				else
					return false;
			}

			//如果当前缓存为空
			else {
				++_IntIndexByDialogSection; //自增索引
			}

			//得到对话信息
			var isEnd = GetDialogInfoRecoder(diaSectionNum, out diaSide, out diaPerson, out diaContent);
			return isEnd;
		}


		/// <summary>得到对话记录 开发思路： 对于输入的“段落编号”，首先在“当前对话数据集合”中进行查询 如果找到，直接返回结果；如果不能找到，则在“全部对话数据集合”中进行查询 实际测试中发现：有时候不能及时找到，显示的还是默认的内容</summary>
		/// <param name="diaSectionNum">输入：段落编号</param>
		/// <param name="diaSide">输出：对话方</param>
		/// <param name="diaPerson">输出：对话人名</param>
		/// <param name="diaContent">输出：对话内容</param>
		/// <returns>true: 输出了合法的对话数据</returns>
		private bool GetDialogInfoRecoder(int diaSectionNum, out DialogSide diaSide, out string diaPerson, out string diaContent) {
			//默认显示的内容
			diaSide = DialogSide.None;
			var strDiaSide = "[GetNextDialogInfoRecoder()/diaSide]";
			diaPerson = "[GetNextDialogInfoRecoder()/diaPerson]";
			diaContent = "[GetNextDialogInfoRecoder()/diaContent]";

			if(diaSectionNum <= 0)
				return false;

			//1: 对于输入的“段落编号”，首先在“当前对话数据集合”中进行查询
			if(_CurDialogCacheArray != null && _CurDialogCacheArray.Count >= 1)
				for(var i = 0; i < _CurDialogCacheArray.Count; i++) //如果段落编号相同
					if(_CurDialogCacheArray[i].DiaSectionNum == diaSectionNum)
						if(_CurDialogCacheArray[i].DiaIndex == _IntIndexByDialogSection) {

							//找到数据，提取数据
							strDiaSide = _CurDialogCacheArray[i].DiaSide;
							//判断对话的讲述者，去掉空格
							if(strDiaSide.Trim().Equals("Hero"))
								diaSide = DialogSide.HeroSide;
							else if(strDiaSide.Trim().Equals("NPC"))
								diaSide = DialogSide.NPCSide;
							diaPerson = _CurDialogCacheArray[i].DiaPerson;
							diaContent = _CurDialogCacheArray[i].DiaContent;

							Log.Write("当前对话数据集合中查找");
							return true;
						}

			//2: 如果不能找到，则在“全部对话数据集合”中进行查询，且把当前段落数据，加入当前的缓存集合
			if(_AllDialogDataArray != null && _AllDialogDataArray.Count >= 1)
				for(var i = 0; i < _AllDialogDataArray.Count; i++) //如果段落编号相同
					if(_AllDialogDataArray[i].DiaSectionNum == diaSectionNum)
						if(_AllDialogDataArray[i].DiaIndex == _IntIndexByDialogSection) {

							//找到数据，提取数据
							strDiaSide = _AllDialogDataArray[i].DiaSide;
							//判断对话的讲述者，去掉空格
							if(strDiaSide.Trim().Equals("Hero"))
								diaSide = DialogSide.HeroSide;
							else if(strDiaSide.Trim().Equals("NPC"))
								diaSide = DialogSide.NPCSide;
							diaPerson = _AllDialogDataArray[i].DiaPerson;
							diaContent = _AllDialogDataArray[i].DiaContent;

							//把当前段落编号中的数据，写入“当前段落缓存集合”
							LoadToCacheArrayBySectionNum(diaSectionNum);

							Log.Write("全部对话数据集合中查找");
							return true;
						}

			//根据当前段落编号，无法查询数据结果，则返回false
			Log.Write("未查找到！");
			return false;
		}


		/*****/

		/// <summary>从对话数据数组中查找</summary>
		/// <param name="dialogDataArray"></param>
		/// <param name="diaSectionNum"></param>
		/// <param name="diaSide"></param>
		/// <param name="diaPerson"></param>
		/// <param name="diaContent"></param>
		/// <param name="strDiaSide"></param>
		/// <returns></returns>
		private bool SearchDialogDataArray(List<DialogDataFormat> dialogDataArray, int diaSectionNum, DialogSide diaSide, string diaPerson, string diaContent, string strDiaSide) {

			if(_AllDialogDataArray != null && _AllDialogDataArray.Count >= 1)
				for(var i = 0; i < _CurDialogCacheArray.Count; i++) //如果段落编号相同
					if(_AllDialogDataArray[i].DiaSectionNum == diaSectionNum)
						if(_AllDialogDataArray[i].DiaIndex == _IntIndexByDialogSection) {
							//找到数据，提取数据
							strDiaSide = _AllDialogDataArray[i].DiaSide;
							//判断对话的讲述者，去掉空格
							if(strDiaSide.Trim().Equals("Hero"))
								diaSide = DialogSide.HeroSide;
							else if(strDiaSide.Trim().Equals("NPC"))
								diaSide = DialogSide.NPCSide;
							diaPerson = _AllDialogDataArray[i].DiaPerson;
							diaContent = _AllDialogDataArray[i].DiaContent;

							//把当前段落编号中的数据，写入“当前段落缓存集合”
							LoadToCacheArrayBySectionNum(diaSectionNum);
							return true;
						}
			return false;
		}


		/// <summary>把当前段落编号中的数据，写入“当前段落缓存集合”</summary>
		/// <param name="diaSectionNum">输入： 当前段落编号</param>
		/// <return>true: 表示操作成功</return>
		private bool LoadToCacheArrayBySectionNum(int diaSectionNum) {

			//输入参数检查
			if(diaSectionNum <= 0)
				return false;

			if(_AllDialogDataArray != null && _AllDialogDataArray.Count >= 1) {
				//当前缓存集合，清空以前的数据
				_CurDialogCacheArray.Clear();

				for(var i = 0; i < _AllDialogDataArray.Count; i++)
					if(_AllDialogDataArray[i].DiaSectionNum == diaSectionNum)
						_CurDialogCacheArray.Add(_AllDialogDataArray[i]);
				return true;
			}
			return false;
		}
	}
}