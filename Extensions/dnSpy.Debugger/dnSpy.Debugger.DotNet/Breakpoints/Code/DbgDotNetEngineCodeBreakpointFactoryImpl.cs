﻿/*
    Copyright (C) 2014-2017 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel.Composition;
using dnSpy.Contracts.Debugger.Breakpoints.Code;
using dnSpy.Contracts.Debugger.DotNet.Breakpoints.Code;
using dnSpy.Contracts.Metadata;

namespace dnSpy.Debugger.DotNet.Breakpoints.Code {
	abstract class DbgDotNetEngineCodeBreakpointFactory2 : DbgDotNetEngineCodeBreakpointFactory {
		public abstract DbgDotNetEngineCodeBreakpoint CreateDotNet(ModuleId module, uint token, uint offset);
	}

	[Export(typeof(DbgDotNetEngineCodeBreakpointFactory))]
	[Export(typeof(DbgDotNetEngineCodeBreakpointFactory2))]
	sealed class DbgDotNetEngineCodeBreakpointFactoryImpl : DbgDotNetEngineCodeBreakpointFactory2 {
		readonly Lazy<DbgCodeBreakpointsService> dbgCodeBreakpointsService;
		readonly BreakpointFormatterService breakpointFormatterService;

		[ImportingConstructor]
		DbgDotNetEngineCodeBreakpointFactoryImpl(Lazy<DbgCodeBreakpointsService> dbgCodeBreakpointsService, BreakpointFormatterService breakpointFormatterService) {
			this.dbgCodeBreakpointsService = dbgCodeBreakpointsService;
			this.breakpointFormatterService = breakpointFormatterService;
		}

		public override DbgCodeBreakpoint Create(ModuleId module, uint token, uint offset, DbgCodeBreakpointSettings bpSettings) =>
			dbgCodeBreakpointsService.Value.Add(new DbgCodeBreakpointInfo(CreateDotNet(module, token, offset), bpSettings));

		public override DbgDotNetEngineCodeBreakpoint CreateDotNet(ModuleId module, uint token, uint offset) {
			var dnbp = new DbgDotNetEngineCodeBreakpointImpl(module, token, offset);
			var formatter = breakpointFormatterService.Create(dnbp);
			dnbp.Formatter = formatter;
			return dnbp;
		}
	}
}
