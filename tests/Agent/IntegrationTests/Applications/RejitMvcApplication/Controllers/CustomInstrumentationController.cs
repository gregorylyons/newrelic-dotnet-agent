﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RejitMvcApplication.Controllers
{
    /// <summary>
    /// Endpoints used for Rejit Integration tests.
    /// </summary>
    public class CustomInstrumentationController : Controller
    {
        /// <summary>
        /// HTTP GET method that has no additional methods to instrument.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String Get()
        {
            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperAddNodeX so that custom instrumentation can be used.
        /// </summary>
        /// <param name="id">Selects which instrumented method to call within the action. Options: 0, 1</param>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetNodeAdd(int id)
        {
            switch (id)
            {
                case 0:
                    CustomMethodDefaultWrapperAddNode();
                    break;
                case 1:
                    CustomMethodDefaultWrapperAddNode1();
                    break;
                default:
                    CustomMethodDefaultWrapperAddNode();
                    break;
            }

            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperDeleteNodeX so that custom instrumentation can be used.
        /// </summary>
        /// <param name="id">Selects which instrumented method to call within the action. Options: 0, 1</param>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetNodeDelete(int id)
        {
            switch (id)
            {
                case 0:
                    CustomMethodDefaultWrapperDeleteNode();
                    break;
                case 1:
                    CustomMethodDefaultWrapperDeleteNode1();
                    break;
                default:
                    CustomMethodDefaultWrapperDeleteNode();
                    break;
            }

            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperAddAttribute so that custom instrumentation can be used.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetAttributeAdd()
        {
            CustomMethodDefaultWrapperAddAttribute();
            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperChangeAttribute so that custom instrumentation can be used.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetAttributeChange()
        {
            CustomMethodDefaultWrapperChangeAttribute();
            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperDeleteAttribute so that custom instrumentation can be used.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetAttributeDelete()
        {
            CustomMethodDefaultWrapperDeleteAttribute();
            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperAddFile so that custom instrumentation can be used.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetFileAdd()
        {
            CustomMethodDefaultWrapperAddFile();
            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperRenameFile so that custom instrumentation can be used.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetFileRename()
        {
            CustomMethodDefaultWrapperRenameFile();
            return "It am working";
        }

        /// <summary>
        /// HTTP GET method that calls CustomMethodDefaultWrapperDeleteFile so that custom instrumentation can be used.
        /// </summary>
        /// <returns>Returns a string containing: It am working</returns>
        [HttpGet]
        public String GetFileDelete()
        {
            CustomMethodDefaultWrapperDeleteFile();
            return "It am working";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperAddNode()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperAddNode1()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperDeleteNode()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperDeleteNode1()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperAddAttribute()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperChangeAttribute()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperDeleteAttribute()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperAddFile()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperRenameFile()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CustomMethodDefaultWrapperDeleteFile()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(5));
        }
    }
}
