namespace GameCreator.Runtime.Common
{
    /// <summary>
    /// 单个符文数据操作
    /// </summary>
    public static class MatrixUtils
    {
        public static void ResetMatrix(bool[,] matrix)
        {
            int n = matrix.GetLength(0); // 假设是一个N x N的矩阵
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    matrix[i, j] = false;
                }
            }
        }
        public static void CloneMatrix(bool[,] matrix, ref bool[,] result)
        {
            ResetMatrix(result);
            int n = matrix.GetLength(0); // 假设是一个N x N的矩阵
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    result[i, j] = matrix[i, j];
                }
            }
        }

        /// <summary>
        ///  右旋90 
        /// </summary>
        public static void ClockwiseRotateMatrix90 (bool[,] matrix, ref bool[,] result)
        {
            ResetMatrix(result);
            int n = matrix.GetLength(0); // 假设是一个N x N的矩阵
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    result[j, n - i - 1] = matrix[i, j];
                }
            }
        }

        /// <summary>
        ///  右旋180度
        /// </summary>
        public static void ClockwiseRotateMatrix180(bool[,] matrix, ref bool[,] result)
        {
            ResetMatrix(result);
            int n = matrix.GetLength(0); // 假设是一个N x N的矩阵
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    result[n - 1 - i, n - j - 1] = matrix[i, j];
                }
            }
        }
        /// <summary>
        /// 右旋270度
        /// 
        /// 
        /// </summary>
        public static void ClockwiseRotateMatrix270(bool[,] matrix, ref bool[,] result)
        {
            ResetMatrix(result);
            int n = matrix.GetLength(0); // 假设是一个N x N的矩阵
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    result[n - j - 1, i] = matrix[i, j];
                }
            }
        }

    }
}
