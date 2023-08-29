﻿//
// Program.cs
//
// Author:
//       JasonXuDeveloper（傑） <jasonxudeveloper@gmail.com>
//
// Copyright (c) 2020 JEngine
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Diagnostics;
using ILRuntime.Runtime;
using JEngine.Core;
using Debug = UnityEngine.Debug;
using System.Net.Sockets;

namespace HotUpdateScripts
{
    public static class Program
    {
        public static void SetupGame()
        {
            Debug.Log("<color=cyan>[SetupGame] 这个周期在ClassBind初始化之前，可以对游戏数据进行一些初始化</color>");
            //防止Task内的报错找不到堆栈，不建议删下面的代码
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                foreach (var innerEx in e.Exception.InnerExceptions)
                {
                    Debug.LogError($"{innerEx.Message}\n" +
                                   $"ILRuntime StackTrace: {innerEx.Data["StackTrace"]}\n\n" +
                                   $"Full Stacktrace: {innerEx.StackTrace}");
                }
            };
        }

        public static void RunGame()
        {
            Debug.Log("<color=yellow>[RunGame] 这个周期在ClassBind初始化后，可以激活游戏相关逻辑</color>");
            //如果生成热更解决方案跳过，参考https://docs.xgamedev.net/zh/documents/0.8/FAQ.html#生成热更工程dll跳过
            //的方法一，把生成的平台改成Any CPU（默认是小写的，windows下无法生成）
        }
    }

    /// <summary>
    /// 测试dll优化
    /// </summary>
    public class Test
    {
        public void DoTest()
        {
            Debug.Log($"original(1) = {Original(1)}");
            Debug.Log($"jit(1) = {JIT(1)}");
            Debug.Log($"optimized(1) = {Optimized(1)}");
            Debug.Log($"optimizedJIT(1) = {OptimizedJIT(1)}");
            RunTest(10);
            RunTest(100);
            RunTest(1000);
            RunTest(10000);
            RunTest(100000);
        }

        public void RunTest(int cnt = 100000)
        {
            Stopwatch sw = new Stopwatch();
            Debug.Log($"{cnt}次计算");
            sw.Start();
            int a = 0;
            int i = cnt;
            while (i-- > 0)
            {
                a = Original(i);
            }

            sw.Stop();
            Debug.Log($"Original: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            i = cnt;
            while (i-- > 0)
            {
                a = JIT(i);
            }

            sw.Stop();
            Debug.Log($"JIT: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            i = cnt;
            while (i-- > 0)
            {
                a = Optimized(i);
            }

            sw.Stop();
            Debug.Log($"Optimized: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            i = cnt;
            while (i-- > 0)
            {
                a = OptimizedJIT(i);
            }

            sw.Stop();
            Debug.Log($"OptimizedJIT: {sw.ElapsedMilliseconds}ms");
        }

        public int Original(int x)
        {
            int a = 5;
            int b = 20;
            int c = 10;
            c = a;
            int d = 5 * a;
            int e = d;
            int f = e / 2;
            int g = f + a + b + c * 6 - b / 4;
            int h = x * 2 + 10 + 3 - 6 - 8 - g;
            return h + 10 % 3;
        }

        [ILRuntimeJIT(ILRuntimeJITFlags.JITImmediately)]
        public int JIT(int x)
        {
            int a = 5;
            int b = 20;
            int c = 10;
            c = a;
            int d = 5 * a;
            int e = d;
            int f = e / 2;
            int g = f + a + b + c * 6 - b / 4;
            int h = x * 2 + 10 + 3 - 6 - 8 - g;
            return h + 10 % 3;
        }

        public int Optimized(int x)
        {
            int a = 5;
            int b = 20;
            int c = 10;
            c = a;
            int d = 5 * a;
            int e = d;
            int f = e / 2;
            int g = f + a + b + c * 6 - b / 4;
            int h = x * 2 + 10 + 3 - 6 - 8 - g;
            return h + 10 % 3;
        }

        [ILRuntimeJIT(ILRuntimeJITFlags.JITImmediately)]
        public int OptimizedJIT(int x)
        {
            int a = 5;
            int b = 20;
            int c = 10;
            c = a;
            int d = 5 * a;
            int e = d;
            int f = e / 2;
            int g = f + a + b + c * 6 - b / 4;
            int h = x * 2 + 10 + 3 - 6 - 8 - g;
            return h + 10 % 3;
        }
    }
}