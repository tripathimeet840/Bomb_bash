// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ZVfo8Wk1K2YIsXSKsoqom54Us4p78WK5fThamP28GZzpuq/P8Lj5BHsg/TZuN3mi/pBkCfD16xsRrmRp1GjFi5Ujb37ifecY8BUUsIOdj05qOpdg1nyUSHFdJAVAFDGyu9NQ3zcATDDPXH3LJTuTpxh9hMwLohGgWapP4TTD2sn483a2UiEMfs+8hD2ZGhQbK5kaERmZGhobha026vrkFcR03STvByzWZzi8uJREnJ5fSFg6BC+guQ2hlzTPGCdH2lJo1V5ErnDubD5cnvAAb0f2rEL3l86hGknbWyuZGjkrFh0SMZ1TnewWGhoaHhsYhq+snDLN3g6V/A8rwCtZ1kMU7inIXfgj5uAnODOlsmPU6WaycgKqT8sZ0z8tXTen9hkYGhsa");
        private static int[] order = new int[] { 6,6,6,13,12,11,8,13,10,10,10,12,13,13,14 };
        private static int key = 82;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
