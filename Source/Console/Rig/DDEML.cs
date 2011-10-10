//=================================================================
// DDEML.cs
//=================================================================
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// ================================================================
//
// Author: S.McClements - WU2X, 7/16/2007
//
// This class setups all the necessary DDEML methods for 
// PowerSDR to communicate with Ham Radio Deluxe
//
//=================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;



namespace PowerSDR
{

    public class DDEML
    {
        /* Predefined Clipboard Formats*/
        public const Int32 CF_TEXT = 1;

        /***** conversation states (usState) *****/
        public const Int32 XST_NULL = 0;  /* quiescent states */
        public const Int32 XST_INCOMPLETE = 1;
        public const Int32 XST_CONNECTED = 2;
        public const Int32 XST_INIT1 = 3;  /* mid-initiation states */
        public const Int32 XST_INIT2 = 4;
        public const Int32 XST_REQSENT = 5;  /* active conversation states */
        public const Int32 XST_DATARCVD = 6;
        public const Int32 XST_POKESENT = 7;
        public const Int32 XST_POKEACKRCVD = 8;
        public const Int32 XST_EXECSENT = 9;
        public const Int32 XST_EXECACKRCVD = 10;
        public const Int32 XST_ADVSENT = 11;
        public const Int32 XST_UNADVSENT = 12;
        public const Int32 XST_ADVACKRCVD = 13;
        public const Int32 XST_UNADVACKRCVD = 14;
        public const Int32 XST_ADVDATASENT = 15;
        public const Int32 XST_ADVDATAACKRCVD = 16;

        /* used in LOWORD(dwData1) of XTYP_ADVREQ callbacks... */
        public const Int32 CADV_LATEACK = 0xFFFF;

        /***** conversation status bits (fsStatus) *****/
        public const Int32 ST_CONNECTED = 0x0001;
        public const Int32 ST_ADVISE = 0x0002;
        public const Int32 ST_ISLOCAL = 0x0004;
        public const Int32 ST_BLOCKED = 0x0008;
        public const Int32 ST_CLIENT = 0x0010;
        public const Int32 ST_TERMINATED = 0x0020;
        public const Int32 ST_INLIST = 0x0040;
        public const Int32 ST_BLOCKNEXT = 0x0080;
        public const Int32 ST_ISSELF = 0x0100;

        /* DDE constants for wStatus field */
        public const Int32 DDE_FACK = 0x8000;
        public const Int32 DDE_FBUSY = 0x4000;
        public const Int32 DDE_FDEFERUPD = 0x4000;
        public const Int32 DDE_FACKREQ = 0x8000;
        public const Int32 DDE_FRELEASE = 0x2000;
        public const Int32 DDE_FREQUESTED = 0x1000;
        public const Int32 DDE_FAPPSTATUS = 0x00ff;
        public const Int32 DDE_FNOTPROCESSED = 0x0000;

        public const Int32 DDE_FACKRESERVED = (DDE_FACK | DDE_FBUSY | DDE_FAPPSTATUS);
        public const Int32 DDE_FADVRESERVED = (DDE_FACKREQ | DDE_FDEFERUPD);
        public const Int32 DDE_FDATRESERVED = (DDE_FACKREQ | DDE_FRELEASE | DDE_FREQUESTED);
        public const Int32 DDE_FPOKRESERVED = (DDE_FRELEASE);

        /***** message filter hook types *****/
        public const Int32 MSGF_DDEMGR = 0x8001;

        /***** codepage constants ****/
        public const Int32 CP_WINANSI = 1004;    /* default codepage for windows & old DDE convs. */
        public const Int32 CP_WINUNICODE = 1200;

        /***** transaction types *****/
        public const Int32 XTYPF_NOBLOCK = 0x0002;  /* CBR_BLOCK will not work */
        public const Int32 XTYPF_NODATA = 0x0004;  /* DDE_FDEFERUPD */
        public const Int32 XTYPF_ACKREQ = 0x0008;  /* DDE_FACKREQ */

        public const Int32 XCLASS_MASK = 0xFC00;
        public const Int32 XCLASS_BOOL = 0x1000;
        public const Int32 XCLASS_DATA = 0x2000;
        public const Int32 XCLASS_FLAGS = 0x4000;
        public const Int32 XCLASS_NOTIFICATION = 0x8000;

        public const Int32 XTYP_ERROR = (0x0000 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK);
        public const Int32 XTYP_ADVDATA = (0x0010 | XCLASS_FLAGS);
        public const Int32 XTYP_ADVREQ = (0x0020 | XCLASS_DATA | XTYPF_NOBLOCK);
        public const Int32 XTYP_ADVSTART = (0x0030 | XCLASS_BOOL);
        public const Int32 XTYP_ADVSTOP = (0x0040 | XCLASS_NOTIFICATION);
        public const Int32 XTYP_EXECUTE = (0x0050 | XCLASS_FLAGS);
        public const Int32 XTYP_CONNECT = (0x0060 | XCLASS_BOOL | XTYPF_NOBLOCK);
        public const Int32 XTYP_CONNECT_CONFIRM = (0x0070 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK);
        public const Int32 XTYP_XACT_COMPLETE = (0x0080 | XCLASS_NOTIFICATION);
        public const Int32 XTYP_POKE = (0x0090 | XCLASS_FLAGS);
        public const Int32 XTYP_REGISTER = (0x00A0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK);
        public const Int32 XTYP_REQUEST = (0x00B0 | XCLASS_DATA);
        public const Int32 XTYP_DISCONNECT = (0x00C0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK);
        public const Int32 XTYP_UNREGISTER = (0x00D0 | XCLASS_NOTIFICATION | XTYPF_NOBLOCK);
        public const Int32 XTYP_WILDCONNECT = (0x00E0 | XCLASS_DATA | XTYPF_NOBLOCK);

        public const Int32 XTYP_MASK = 0x00F0;
        public const Int32 XTYP_SHIFT = 4;  /* shift to turn XTYP_ into an index */


        ////////////////////////////////////
        // DDE Native Method Declarations //
        ////////////////////////////////////


        // *** Not currently used or tested ***
        // BOOL DdeAbandonTransaction(
        //    DWORD idInst,
        //    HCONV hConv,
        //    DWORD idTransaction
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeAbandonTransaction(Int32 idInst,IntPtr hConv,Int32 idTransaction);

        // *** Not currently used or tested ***
        // LPBYTE DdeAccessData(      
        //    HDDEDATA hData,
        //    LPDWORD pcbDataSize
        // );
        // [DllImport("user32.dll")]
        //public static extern byte DdeAccessData(IntPtr hData, UInt32 pcbDataSize);

        // *** Not currently used or tested ***
        // HDDEDATA DdeAddData(      
        //    HDDEDATA hData,
        //    LPBYTE pSrc,
        //    DWORD cb,
        //    DWORD cbOff
        // );
        // [DllImport("user32.dll")]
        // public static extern IntPtr DdeAddData(IntPtr hData, byte pSrc, Int32 cb, Int32 cbOff);

        // HDDEDATA CALLBACK DdeCallback(      
        //    UINT uType,
        //    UINT uFmt,
        //    HCONV hconv,
        //    HDDEDATA hsz1,
        //    HDDEDATA hsz2,
        //    HDDEDATA hdata,
        //    HDDEDATA dwData1,
        //    HDDEDATA dwData2
        // );
        public delegate IntPtr DdeCallback(Int32 uType, Int32 uFmt, IntPtr hconv, IntPtr hsz1, IntPtr hsz2, IntPtr hdata, IntPtr dwData1, IntPtr dwData2);

        // HDDEDATA DdeClientTransaction(      
        //    LPBYTE pData,
        //    DWORD cbData,
        //    HCONV hConv,
        //    HSZ hszItem,
        //    UINT wFmt,
        //    UINT wType,
        //    DWORD dwTimeout,
        //    LPDWORD pdwResult
        // );
        [DllImport("user32.dll")]
        public static extern IntPtr DdeClientTransaction(byte[] pData, uint cbData, IntPtr hConv, IntPtr hszItem, Int32 wFmt, Int32 wType, Int32 dwTimeout, ref Int32 pdwResult);

        // *** Not currently used or tested ***
        // int DdeCmpStringHandles(      
        //     HSZ hsz1,
        //     HSZ hsz2
        // );
        // [DllImport("user32.dll")]
        // public static extern Int32 DdeCmpStringHandles(IntPtr hsz1, IntPtr hsz2);

        // HCONV DdeConnect(      
        //    DWORD idInst,
        //    HSZ hszService,
        //    HSZ hszTopic,
        //    PCONVCONTEXT pCC
        // );
        [DllImport("user32.dll")]
        public static extern IntPtr DdeConnect(Int32 idInst, IntPtr hszService, IntPtr hszTopic, IntPtr pCC);

        // *** Not currently used or tested ***
        // HCONVLIST DdeConnectList(      
        //    DWORD idInst,
        //    HSZ hszService,
        //    HSZ hszTopic,
        //    HCONVLIST hConvList,
        //    PCONVCONTEXT pCC
        // );
        // [DllImport("user32.dll")]
        // public static extern IntPtr DdeConnectList(Int32 idInst, IntPtr hszService, IntPtr hszTopic, IntPtr ConvList, IntPtr pCC);

        // *** Not currently used or tested ***
        // HDDEDATA DdeCreateDataHandle(      
        //    DWORD idInst,
        //    LPBYTE pSrc,
        //    DWORD cb,
        //    DWORD cbOff,
        //    HSZ hszItem,
        //    UINT wFmt,
        //    UINT afCmd
        // );
        // [DllImport("user32.dll")]
        // public static extern IntPtr DdeCreateDataHandle(Int32 idInst, byte pSrc, Int32 cb, Int32 cbOff, IntPtr hszItem, Int32 wFmt, Int32 afCmd);

        // HSZ DdeCreateStringHandle(      
        //    DWORD idInst,
        //    LPTSTR psz,
        //    int iCodePage
        // );
        [DllImport("user32.dll")]
        public static extern IntPtr DdeCreateStringHandle(Int32 idInst, IntPtr psz, Int32 iCodePage);

        // BOOL DdeDisconnect(      
        //    HCONV hConv
        // );
        [DllImport("user32.dll")]
        public static extern Boolean DdeDisconnect(IntPtr hConv);

        // *** Not currently used or tested ***
        // BOOL DdeDisconnectList(      
        //    HCONVLIST hConvList
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeDisconnectList(IntPtr hConvList);

        // *** Not currently used or tested ***
        // BOOL DdeEnableCallback(
        //    DWORD idInst,
        //    HCONV hConv,
        //    UINT wCmd
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeEnableCallback(Int32 idInst, IntPtr hConv, Int32 wCmd);

        // BOOL DdeFreeDataHandle(
        //    HDDEDATA hData
        // );
        [DllImport("user32.dll")]
        public static extern Boolean DdeFreeDataHandle(IntPtr hData);

        // BOOL DdeFreeStringHandle(
        //    DWORD idInst,
        //    HSZ hsz
        // );
        [DllImport("user32.dll")]
        public static extern Boolean DdeFreeStringHandle(Int32 idInst, IntPtr hsz);

        // DWORD DdeGetData(
        //    HDDEDATA hData,
        //    LPBYTE pDst,
        //    DWORD cbMax,
        //    DWORD cbOff
        // );
        [DllImport("user32.dll")]
        public static extern Int32 DdeGetData(IntPtr hData, byte[] pDst, Int32 cbMax, Int32 cbOff);

        // UINT DdeGetLastError(
        //    DWORD idInst
        // );
        [DllImport("user32.dll")]
        public static extern Int32 DdeGetLastError(Int32 idInst);

        // *** Not currently used or tested ***
        // BOOL DdeImpersonateClient(
        //    HCONV hConv
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeImpersonateClient(IntPtr hConv);

        // UINT DdeInitialize(
        //    LPDWORD pidInst,
        //    PFNCALLBACK pfnCallback,
        //    DWORD afCmd,
        //    DWORD ulRes
        // );
        [DllImport("user32.dll")]
        public static extern Int32 DdeInitialize(ref Int32 pidInst, DdeCallback pfnCallback, Int32 afCmd, Int32 ulRes);

        // *** Not currently used or tested ***
        // BOOL DdeKeepStringHandle(
        //    DWORD idInst,
        //    HSZ hsz
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeKeepStringHandle(Int32 idInst, IntPtr hsz);

        // *** Not currently used or tested ***
        // HDDEDATA DdeNameService(
        //    DWORD idInst,
        //    UINT hsz1,
        //    UINT hsz2,
        //    UINT afCmd
        // );
        // [DllImport("user32.dll")]
        // public static extern IntPtr DdeNameService(Int32 idInst, Int32 hsz1, Int32 hsz2, Int32 afCmd);

        // *** Not currently used or tested ***
        // BOOL DdePostAdvise(
        //    DWORD idInst,
        //    HSZ hszTopic,
        //    HSZ hszItem
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdePostAdvise(Int32 idInst, IntPtr hszTopic, IntPtr hszItem);

        // *** Not currently used or tested ***
        // UINT DdeQueryConvInfo(
        //    HCONV hConv,
        //    DWORD idTransaction,
        //    PCONVINFO pConvInfo
        // );
        // [DllImport("user32.dll")]
        // public static extern Int32 DdeQueryConvInfo(IntPtr idInst, Int32 idTransaction, IntPtr pConvInfo);

        // *** Not currently used or tested ***
        // HCONV DdeQueryNextServer(
        //    HCONVLIST hConvList,
        //    HCONV hConvPrev
        // );
        // [DllImport("user32.dll")]
        // public static extern IntPtr DdeQueryNextServer(IntPtr hConvList, IntPtr hConvPrev);

        // DWORD DdeQueryString(
        //    DWORD idInst,
        //    HSZ hsz,
        //    LPTSTR psz,
        //    DWORD cchMax,
        //    int iCodePage
        // );
        [DllImport("user32.dll")]
        public static extern Int32 DdeQueryString(Int32 idInst, IntPtr hsz, StringBuilder psz, Int32 cchMax, Int32 iCodePage);

        // *** Not currently used or tested ***
        // HCONV DdeReconnect(
        //    HCONV hConv
        // );
        [DllImport("user32.dll")]
        public static extern IntPtr DdeReconnect(IntPtr hConv);

        // *** Not currently used or tested ***
        // BOOL DdeSetUserHandle(
        //    HCONV hConv,
        //    DWORD id,
        //    DWORD_PTR hUser
        // };
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeSetUserHandle(IntPtr hConv, Int32 id, IntPtr hUser);

        // *** Not currently used or tested ***
        // BOOL DdeUnaccessData(      
        //    HDDEDATA hData
        // );
        // [DllImport("user32.dll")]
        // public static extern Boolean DdeUnaccessData(IntPtr hData);

        // BOOL DdeUninitialize(      
        //    DWORD idInst
        // );
        [DllImport("user32.dll")]
        public static extern Boolean DdeUninitialize(Int32 idInst);
    }
}
