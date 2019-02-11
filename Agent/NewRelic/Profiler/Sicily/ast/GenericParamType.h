#pragma once
#include <memory>
#include "Type.h"

namespace sicily
{
	namespace ast
	{
		class GenericParamType : public Type
		{
		public:
			enum GenericParamKind
			{
				kTYPE = 0x13,
				kMETHOD = 0x1e,
			};

			GenericParamType(GenericParamKind kind, uint32_t number);
			virtual ~GenericParamType() { }

			virtual GenericParamKind GetGenericParamKind();
			virtual uint32_t GetNumber();

			virtual xstring_t ToString() const override;

		private:
			GenericParamKind kind_;
			uint32_t number_;
		};

		typedef std::shared_ptr<GenericParamType> GenericParamTypePtr;
	}
}
