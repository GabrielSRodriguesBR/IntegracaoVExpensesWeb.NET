using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace IntegracaoVExpensesWeb.Business
{
	public class ExcelWriter
	{
		private int CURRENT_ROW = 0;
		private string FILE_NAME = "unknown";

		private class HeaderCells
		{
			public string propName { get; set; }
			public PropertyInfo propInfo { get; set; }
		}

		/// <summary>
		/// Construtor ExcelWriter
		/// </summary>
		/// <param name="fileName">Nome do arquivo .xlsx</param>

		public ExcelWriter(string fileName)
		{
			this.FILE_NAME = fileName;
		}


		/// <summary>
		/// Cria um novo workbook Excel a partir de uma lista de dados
		/// </summary>
		/// <typeparam name="T">Model Class da lista de dados</typeparam>
		/// <param name="data">Lista de dados</param>
		/// <returns>Interface workbook Excel</returns>
		public IWorkbook Create<T>(IEnumerable<T> data)
		{
			IWorkbook workbook = new XSSFWorkbook();
			ISheet sheet = workbook.CreateSheet(FILE_NAME);
			IRow row = this.AddRow(sheet);

			#region criar cabeçalhos

			List<HeaderCells> headerCells = this.GetHeaders(typeof(T));

			for (int i = 0; i < headerCells.Count; i++)
				row.CreateCell(i).SetCellValue(headerCells.ElementAt(i).propName);

			//Customizar cor do cabeçalho
			ICellStyle headerStyle = workbook.CreateCellStyle();
			headerStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
			headerStyle.FillPattern = FillPattern.SolidForeground;
			row.RowStyle = headerStyle;

			#endregion

			#region criar linhas
			foreach (var item in data)
			{
				row = this.AddRow(sheet);
				for (int i = 0; i < headerCells.Count(); i++)
				{
					PropertyInfo propInfo = headerCells.ElementAt(i).propInfo;

					if (item.GetType().GetProperty("IsLoaded")?.GetValue(item) as bool? == false)
						item.GetType().GetMethod("LoadData")?.Invoke(item, null);

					object obj = propInfo.GetValue(item);

					Type propType = propInfo.PropertyType;

					//Tratamento de tipos nullable
					if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
						propType = Nullable.GetUnderlyingType(propInfo.PropertyType);

					if (IsNumericType(propType))
						row.CreateCell(i).SetCellValue(Convert.ToDouble(obj));
					else
						row.CreateCell(i).SetCellValue(Convert.ToString(obj));
				}
			}
			#endregion

			//adicionar o autofiltro nas colunas
			sheet.SetAutoFilter(new CellRangeAddress(0, sheet.LastRowNum, 0, headerCells.Count() - 1));

			// Ajustar automaticamente o tamanho das colunas de acordo com o conteúdo
			for (int i = 0; i < headerCells.Count; i++)
			{
				sheet.AutoSizeColumn(i);

			}
			return workbook;
		}


		/// <summary>
		/// Cria um novo workbook Excel a partir de uma lista de dados e escreve em uma Resposta HTTP
		/// </summary>
		/// <typeparam name="T">Model Class da lista de dados</typeparam>
		/// <param name="data">Lista de dados</param>
		/// <param name="responseOutput">Resposta HTTP</param>
		public void Write<T>(IEnumerable<T> data, HttpResponseBase responseOutput)
		{
			IWorkbook workbook = this.Create(data);
			Write(workbook, responseOutput);
		}

		/// <summary>
		/// Cria um novo workbook Excel a partir de uma lista de dados e escreve em um diretório
		/// </summary>
		/// <typeparam name="T">Model Class da lista de dados</typeparam>
		/// <param name="data">Lista de dados</param>
		/// <param name="path">Diretório do arquivo</param>
		public void Write<T>(IEnumerable<T> data, string path)
		{
			IWorkbook workbook = this.Create(data);
			Write(workbook, path);
		}

		/// <summary>
		/// Escreve um workbook Excel em uma Resposta HTTP
		/// </summary>
		/// <param name="workbook">Workbook Excel</param>
		/// <param name="responseOutput">Resposta HTTP</param>
		public void Write(IWorkbook workbook, HttpResponseBase responseOutput)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				workbook.Write(memoryStream);

				responseOutput.Clear();
				responseOutput.Buffer = true;
				responseOutput.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				responseOutput.AddHeader("Content-Disposition", $"attachment; filename={FILE_NAME}.xlsx");
				responseOutput.BinaryWrite(memoryStream.ToArray());
				responseOutput.End();
			}
		}

		/// <summary>
		/// Escreve um workbook Excel em um diretório
		/// </summary>
		/// <param name="workbook">Workbook Excel</param>
		/// <param name="path">Diretório do arquivo</param>
		public void Write(IWorkbook workbook, string path)
		{
			using (FileStream fileStream = new FileStream(Path.Combine(path, $"{FILE_NAME}.xlsx"), FileMode.Create, FileAccess.Write))
				workbook.Write(fileStream);
		}


		/// <summary>
		/// Adiciona uma nova linha em um ISheet
		/// </summary>
		/// <param name="sheet">ISheet</param>
		/// <returns>Interface IRow</returns>
		private IRow AddRow(ISheet sheet)
		{
			IRow newRow = sheet.CreateRow(CURRENT_ROW);
			CURRENT_ROW++;
			return newRow;
		}

		/// <summary>
		/// Obtém a descrição de todas as propriedades de um tipo
		/// </summary>
		/// <param name="type">Tipo de uma Classe</param>
		/// <returns>Lista de propriedades</returns>
		private List<HeaderCells> GetHeaders(Type type)
		{
			List<HeaderCells> returnList = new List<HeaderCells>();
			//ignorar propriedades virtuais ou não mapeadas de models
			foreach (var item in type.GetProperties().Where(p => !p.GetGetMethod().IsVirtual && !Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute))))
			{
				DisplayAttribute attribute = (DisplayAttribute)Attribute.GetCustomAttribute(item, typeof(DisplayAttribute), true);

				string headerName = string.Empty;

				if (item.GetCustomAttribute<DisplayAttribute>()?.ResourceType != null)
				{
					System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager(item.GetCustomAttribute<DisplayAttribute>().ResourceType);
					headerName = resourceManager.GetString(attribute.Name);
				}
				else if (item.GetCustomAttribute<DisplayAttribute>() != null)
					headerName = attribute.Name;
				else
					headerName = item.Name;

				returnList.Add(new HeaderCells
				{
					propName = headerName,
					propInfo = item
				});
			}

			return returnList;
		}

		/// <summary>
		/// Verifica se um tipo é um número
		/// </summary>
		/// <param name="type">Tipo da Propriedade</param>
		/// <returns>TRUE se é um número, FALSE se ao contrário</returns>
		private bool IsNumericType(Type type)
		{
			return type == typeof(int) || type == typeof(double) || type == typeof(float) || type == typeof(decimal)
				   || type == typeof(long) || type == typeof(short) || type == typeof(byte) || type == typeof(sbyte)
				   || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort);
		}


	}
}