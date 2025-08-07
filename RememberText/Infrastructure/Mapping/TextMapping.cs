using RememberText.Domain.Entities;
using RememberText.Infrastructure.Helpers;
using RememberText.Models;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RememberText.Infrastructure.Mapping
{
    public static class TextMapping
    {
        public static ProjectTextViewModel ProjectTextToView(ProjectWithText projecttext) => new ProjectTextViewModel
        {
            TopicId = projecttext.TopicId,
            TopicTitle = projecttext.TopicTitle,
            CopyrightName = projecttext.Copyright == null ? "" : projecttext.Copyright.CopyrightName,
            SourceLang = projecttext.SourceLang,
            TargetLang = projecttext.TargetLang,
            HowToDisplay = projecttext.HowToDisplay,
            SourceText = projecttext.SourceText,
            TargetText = projecttext.TargetText
        };

        public static PracticeSyncViewModel ProjectToView(this ProjectWithText projecttext)
        {
            PracticeSyncViewModel textToView = new PracticeSyncViewModel();
            textToView.TopicId = projecttext.TopicId;
            textToView.TopicTitle = projecttext.TopicTitle;
            textToView.SourceLang = projecttext.SourceLang;
            textToView.TargetLang = projecttext.TargetLang;
            textToView.SourceTextId = projecttext.SourceTextId;
            textToView.SourceText = projecttext.SourceText;
            textToView.TargetTextId = projecttext.TargetTextId;
            textToView.TargetText = projecttext.TargetText;
            textToView.HowToDisplay = projecttext.HowToDisplay;

            return textToView;
        }

        public static EditProjectViewModel EditProjectToView(this ProjectWithText projecttext, string WhatToEdit = "")
        {
            // The editing text is always target text in the edit mode!
            EditProjectViewModel projectToView = new EditProjectViewModel();
            projectToView.TopicId = projecttext.TopicId;
            projectToView.TopicTitle = projecttext.TopicTitle;
            if(projecttext.Copyright != null)
            {
                projectToView.CopyrightId = projecttext.Copyright.Id;
                projectToView.CopyrightName = projecttext.Copyright.CopyrightName;
            }

            if(!string.IsNullOrEmpty(projecttext.TargetLang))
            {
                string sourceLang;
                string targetLang;

                if(WhatToEdit == "source")
                {
                    sourceLang = projecttext.TargetLang;
                    targetLang = projecttext.SourceLang;
                    projectToView.SourceLangId = projecttext.TargetLangId;
                    projectToView.SourceTextId = (int)projecttext.TargetTextId;
                    projectToView.TargetLangId = projecttext.SourceLangId;
                    projectToView.TargetTextId = projecttext.SourceTextId;
                    projectToView.SourceText = projecttext.TargetText;
                    projectToView.TargetText = projecttext.SourceText;
                }
                else
                {
                    sourceLang = projecttext.SourceLang;
                    targetLang = projecttext.TargetLang;
                    projectToView.SourceLangId = projecttext.SourceLangId;
                    projectToView.SourceTextId = projecttext.SourceTextId;
                    projectToView.TargetLangId = projecttext.TargetLangId;
                    projectToView.TargetTextId = (int)projecttext.TargetTextId;
                    projectToView.SourceText = projecttext.SourceText;
                    projectToView.TargetText = projecttext.TargetText;
                }

                projectToView.SourceLang = sourceLang;
                projectToView.TargetLang = targetLang;
            }
            else
            {
                projectToView.TargetLang = projecttext.SourceLang;
                projectToView.TargetTextId = projecttext.SourceTextId;
                projectToView.TargetLangId = projecttext.SourceLangId;
                projectToView.TargetText = projecttext.SourceText;
            }

            return projectToView;
        }

        public static UpdateSentenceFormText TextsToDeleteSentence(ProjectWithText projecttext, int sentenceindex) => new UpdateSentenceFormText
        {
            SentenceIndex = sentenceindex,
            SourceTextId = projecttext.SourceTextId,
            SourceLang = projecttext.SourceLang,
            SourceText = projecttext.SourceText,
            TargetTextId = projecttext.TargetTextId,
            TargetLang = projecttext.TargetLang,
            TargetText = projecttext.TargetText
        };
    }
}
