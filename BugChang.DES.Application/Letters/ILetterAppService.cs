﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugChang.DES.Application.Letters.Dtos;
using BugChang.DES.Core.Commons;
using BugChang.DES.Core.Exchanges.Channel;
using BugChang.DES.Core.Letters;

namespace BugChang.DES.Application.Letters
{
    public interface ILetterAppService
    {

        Task<string> GetBarcodeNoByLetterId(int letterId);

        Task<ReceiveLetterEditDto> GetReceiveLetter(int letterId);

        Task<LetterSendEditDto> GetSendLetter(int letterId);

        Task<ResultEntity> AddReceiveLetter(ReceiveLetterEditDto receiveLetter);

        Task<PageResultModel<LetterReceiveListDto>> GetTodayReceiveLetters(PageSearchCommonModel pageSearchModel);

        Task<LetterReceiveBarcodeDto> GetReceiveBarcode(int letterId);

        Task<LetterSendBarcodeDto> GetSendBarcode(int letterId);

        Task<PageResultModel<LetterReceiveListDto>> GetReceiveLetters(LetterPageSerchModel pageSearchModel);

        Task<PageResultModel<LetterReceiveListDto>> GetSearchLetters(LetterPageSerchModel pageSerchModel);

        Task<ResultEntity> AddSendLetter(LetterSendEditDto sendLetter);

        Task<PageResultModel<LetterSendListDto>> GetTodaySendLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<LetterSendListDto>> GetSendLetters(LetterPageSerchModel pageSearchModel);

        Task<PageResultModel<LetterBackListDto>> GetBackLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<LetterBackListDto>> GetBackLettersForSearch(PageSearchCommonModel pageSearchModel);

        Task<ResultEntity> BackLetter(int letterId, int departmentId, int operatorId);

        Task<PageResultModel<LetterCancelListDto>> GetCancelLetters(PageSearchCommonModel pageSearchModel);

        Task<PageResultModel<LetterCancelListDto>> GetCancelLettersForSearch(PageSearchCommonModel pageSearchModel);

        Task<ResultEntity> CancelLetter(int letterId, int departmentId, int operatorId, int applicantId);

        Task<PageResultModel<LetterSortingDto>> GetNoSortingLetters(EnumChannel channel);

        Task<ResultEntity> CreateSortingList(EnumChannel channel, List<int> letterIds, int userId);

        Task<ResultEntity> Change2Jytx(int letterId);

        Task<ResultEntity> GetWriteCpuCardData(int listId);

        Task<IList<LetterSortingDto>> GetSortListDetails(int listId);

        Task<int> GetLetterIdByBarcodeNo(string barcodeNo);

        Task<SortingListDto> GetSortingList(int listId);

        Task<PageResultModel<SortingListDto>> GetSortingLists(PageSearchCommonModel pageSearch);

        Task<ResultEntity> GetCheckInfo(int userId, int placeId, DateTime beginTime, DateTime endTime);

        Task<PageResultModel<LetterCheckListDto>> GetCheckLetters(PageSearchDetailModel pageSearch);

        Task<PageResultModel<LetterReceiveListDto>> Out2InsideLetters(PageSearchCommonModel pageSearch);

        Task<DepartmentStatisticsDto> GetDepartmentStatistics(int departmentId, DateTime beginDate, DateTime endDate);

        Task<Dictionary<string, int>> GetPlaceStatistics(DateTime beginDate, DateTime endDate);

        Task<IList<ExchangeLogListDto>> GetExchangeLogs(string barcodeNo);
    }
}
