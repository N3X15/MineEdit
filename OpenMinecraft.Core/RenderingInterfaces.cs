/*
 * Created by SharpDevelop.
 * User: Rob
 * Date: 8/20/2010
 * Time: 2:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace OpenMinecraft
{
	/// <summary>
	/// Description of RenderingInterfaces.
	/// </summary>
	public interface IChunkRenderer
	{
		void RenderGround();
		void RenderWater();
	}
}
